using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProjectLibrary.Data;
using ProjectLibrary.Data.Entities;
using ProjectLibrary.Models;
using ProjectLibrary.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using Token.API.Models;

namespace Token.API.Contracts
{
    public class TokenCheckRequest
    {
        private readonly DBContext _context;
        private readonly AppSettings _appSettings;

        public TokenCheckRequest(DBContext context, AppSettings appSettings)
        {
            _context = context;
            _appSettings = appSettings;
        }

        public async Task<ClaimsPrincipal?> Check(string token, string userAgent) 
        {
            JwtSecurityToken? jwtSecurityToken = GetJwtSecurityToken(token);
            if( jwtSecurityToken == null) return null;
            string? salt = GetSaltFromToken(jwtSecurityToken);
            if( salt == null ) return null;
            TokenData? tokenData = await GetTokenData(salt);
            if( tokenData == null ) return null;
            TokenOptionsModel options = new TokenOptionsModel();
            if (int.TryParse(salt, out int value))
            {
                options.SaltID = value;
            }
            else 
            {
                throw new InvalidOperationException($"SaltID invalid: {salt}, {tokenData.ID}");
            }
            options.Token = tokenData.Token;
            if (_appSettings.Secret == null) throw new ArgumentNullException("Secret key null");
            options.SecretKey = _appSettings.Secret;
            options.Created = tokenData.TokenCreated;
            options.UserAgent = userAgent;
            SymmetricSecurityKey key = options.GetSymmetricSecurityKey();

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = _appSettings.Issuer,

                ValidateAudience = true,
                ValidAudience = _appSettings.Audience,

                ValidateLifetime = false,

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key
            };
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                ClaimsPrincipal principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                return principal;
            }
            catch
            {
                return null;
            }
        }

        public JwtSecurityToken? GetJwtSecurityToken(string token)
        {
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            JwtSecurityToken? jsonToken = handler.ReadToken(token) as JwtSecurityToken;
            return jsonToken;
        }

        public async Task<TokenData?> GetTokenData (string salt)
        {
            int saltID = int.Parse(salt);
            TokenData? tokenData = await _context.TokensData.FindAsync(saltID);
            return tokenData;
        }

        public string? GetSaltFromToken(JwtSecurityToken token)
        {
            Claim? saltClaim = token.Claims.FirstOrDefault(c => c.Type == "salt_id");
            string? salt = saltClaim?.Value;
            return salt;
        }
    }
}
