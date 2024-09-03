using BookingLibrary.Data;
using BookingLibrary.Data.DAO;
using BookingLibrary.JsonResponce;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Token.API.Models;
using Token.API.Services;

namespace Token.API.Contracts
{
    public class TokenGenerationRequest
    {
        public TokenDBContext Context { get; private set; }

        private TokenDAO? _tokenData;
        public TokenDAO TokenData
        {
            get
            {
                if (_tokenData == null) throw ResponseFormat.TOKEN_DATA_NULL.Exception;
                return _tokenData;
            }
        }

        private TokenGenerationResponse? _tokenGenerationResponse;
        public TokenGenerationResponse TokenGenerationResponse
        {
            get
            {
                if (_tokenGenerationResponse == null) throw ResponseFormat.TOKEN_GENERATION_RESPOSE_ERROR.Exception;
                return _tokenGenerationResponse;
            }
        }

        public TokenGenerationRequest(TokenDBContext dBContext)
        {
            Context = dBContext;
        }


        public async Task<TokenGenerationRequest> GetToken(string userAgent, string secret, string issuer, string audience)
        {
            TokenDAO tokenData = await TokenDBContext.GenerateTokenAsync(Context);
            TokenOptionsModel options = new();
            options.UserAgent = userAgent;
            options.TokenID = tokenData.TokenID;
            options.SecretKey = secret;
            options.Salt = tokenData.Salt;
            options.Created = tokenData.TokenCreated;

            _tokenGenerationResponse = new TokenGenerationResponse()
            {
                token = GenerateToken(GenerateIdentity(options.TokenID), options, issuer, audience),
                salt = options.Salt,
            };
            return this;
        }

        private string GenerateToken(ClaimsIdentity identity, TokenOptionsModel optionsModel, string issuer, string audience)
        {
            var jwt = new JwtSecurityToken(
                  issuer: issuer,
                  audience: audience,
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
