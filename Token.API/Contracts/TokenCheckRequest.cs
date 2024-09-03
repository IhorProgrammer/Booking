using BookingLibrary.Data;
using BookingLibrary.Data.DAO;
using BookingLibrary.JsonResponce;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Token.API.Models;

namespace Token.API.Contracts
{
    public class TokenCheckRequest
    {
        private readonly TokenDBContext _context;

        private string token = "";

        public string Token
        {
            get { return token; }

            set 
            {
                token = value; 
            }
        }

        private TokenDAO? _tokenData;
        public TokenDAO TokenData { 
            get 
            {
                if (_tokenData != null) return _tokenData;
                _tokenData = TokenDBContext.GetTokenDataAsync(_context, TokenId).Result;
                if (_tokenData != null) return _tokenData;
                throw ResponseFormat.TOKEN_DATA_NULL.Exception;
            }
        }

        private ClaimsPrincipal? _principal;
        public ClaimsPrincipal Principal
        {
            get
            {
                if (_principal == null) throw ResponseFormat.TOKEN_DATA_NULL.Exception;
                return _principal;
            }
        }

        private string tokenId = ""; 
        public string TokenId
        {
            get
            {
                if (!string.IsNullOrEmpty(tokenId)) return tokenId;

                JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
                JwtSecurityToken jwtToken = handler.ReadJwtToken(Token);
                string? tokenIdTemp = jwtToken.Claims.FirstOrDefault(c => c.Type == "token_id")?.Value;
                if( tokenIdTemp == null ) throw ResponseFormat.TOKEN_ID_INVALID.Exception;
                tokenId = tokenIdTemp;
                return tokenId;
            }
        }



        public TokenCheckRequest(TokenDBContext context)
        {
            _context = context;
        }

        public TokenCheckRequest Check(string token, string userAgent, string secret, string issuer, string audience) 
        {
            Token = token;
            
            TokenOptionsModel options = new TokenOptionsModel();
            options.SecretKey = secret;
            options.TokenID = TokenId;
            options.Salt = TokenData.Salt;
            options.Created = TokenData.TokenCreated;
            options.UserAgent = userAgent;
            SymmetricSecurityKey key = options.GetSymmetricSecurityKey();

            TokenValidationParameters validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = issuer,

                ValidateAudience = true,
                ValidAudience = audience,

                ValidateLifetime = false,

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key
            };
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                ClaimsPrincipal principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                _principal = principal;
            }
            catch (Exception)
            {
                throw ResponseFormat.TOKEN_INVALID.Exception;
            }
            return this;
        }
    }
}
