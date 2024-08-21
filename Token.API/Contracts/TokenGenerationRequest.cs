using Microsoft.IdentityModel.Tokens;
using ProjectLibrary.Data;
using ProjectLibrary.Data.Entities;
using ProjectLibrary.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Token.API.Models;

namespace Token.API.Contracts
{
    public class TokenGenerationRequest
    {
        private readonly DBContext _context;
        private readonly AppSettings _appSettings;
        public TokenGenerationRequest(DBContext context, AppSettings appSettings)
        {
            _appSettings = appSettings;
            _context = context;

            if (String.IsNullOrEmpty(_appSettings.Issuer)) throw new ArgumentNullException("Issuer null", nameof(_appSettings.Issuer));
            if (String.IsNullOrEmpty(_appSettings.Audience)) throw new ArgumentNullException("Audience null", nameof(_appSettings.Issuer));
            if (_appSettings.Secret == null) throw new ArgumentNullException("Scret key null", nameof(_appSettings.Secret));
        }

        public async Task<TokenData> AddTokenToDB()
        {
            TokenData tokenData = new TokenData();
            tokenData.TokenID = Guid.NewGuid().ToString();
            tokenData.Salt = Guid.NewGuid().ToString();
            tokenData.TokenCreated = DateTime.UtcNow;
            tokenData.TokenUsed = DateTime.UtcNow;
            tokenData.UserID = null;
            await _context.TokensData.AddAsync(tokenData);
            await _context.SaveChangesAsync();
            return tokenData;
        }

        public async Task<TokenGenerationResponce> GetToken(string userAgent)
        {


            TokenData tokenData = await AddTokenToDB();
            TokenOptionsModel options = new();
            options.UserAgent = userAgent;
            options.TokenID = tokenData.TokenID;
            if (_appSettings.Secret == null) throw new ArgumentNullException("Scret key null");
            options.SecretKey = _appSettings.Secret;
            options.Salt = tokenData.Salt;
            options.Created = tokenData.TokenCreated;

            return new TokenGenerationResponce()
            {
                token = GenerateToken(GenerateIdentity(options.TokenID), options),
                salt = options.Salt,
            };
            }

        private string GenerateToken(ClaimsIdentity identity, TokenOptionsModel optionsModel)
        {
            var jwt = new JwtSecurityToken(
                  issuer: _appSettings.Issuer,
                  audience: _appSettings.Audience,
                  notBefore: optionsModel.Created,
                  claims: identity.Claims,
                  signingCredentials: new SigningCredentials(optionsModel.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            return encodedJwt;
        }

        private ClaimsIdentity GenerateIdentity(string token_id)
        {
            var claims = new List<Claim>
            {
                new Claim("token_id", token_id),
            };
            ClaimsIdentity claimsIdentity =
            new ClaimsIdentity(claims, "Token");
            return claimsIdentity;
        }
        
        
    }
}
