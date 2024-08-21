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
            string? tokenId = GetTokenIdFromToken(jwtSecurityToken);
            if( tokenId == null ) return null;
            TokenData? tokenData = await GetTokenData(tokenId);
            if( tokenData == null ) return null;
            
            if (_appSettings.Secret == null) throw new ArgumentNullException("Secret key null");
            TokenOptionsModel options = new TokenOptionsModel();
            options.SecretKey = _appSettings.Secret;
            options.TokenID = tokenId;
            options.Salt = tokenData.Salt;
            options.Created = tokenData.TokenCreated;
            options.UserAgent = userAgent;
            SymmetricSecurityKey key = options.GetSymmetricSecurityKey();

            TokenValidationParameters validationParameters = new TokenValidationParameters
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

        public string? GetSalt(string token)
        {
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            JwtSecurityToken jwtToken = handler.ReadJwtToken(token);
            string? salt = jwtToken.Claims.FirstOrDefault(c => c.Type == "token_id")?.Value;
            return salt;
        }

        public JwtSecurityToken? GetJwtSecurityToken(string token)
        {
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            JwtSecurityToken? jsonToken = handler.ReadToken(token) as JwtSecurityToken;
            return jsonToken;
        }

        public async Task<TokenData?> GetTokenData (string tokenId)
        {
            TokenData? tokenData = await _context.TokensData.FindAsync(tokenId);
            if(tokenData == null) return null;
            tokenData.TokenUsed = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return tokenData;
        }

        public string? GetTokenIdFromToken(JwtSecurityToken token)
        {
            Claim? saltClaim = token.Claims.FirstOrDefault(c => c.Type == "token_id");
            string? salt = saltClaim?.Value;
            return salt;
        }
    }
}
