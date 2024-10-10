using BookingLibrary.JsonResponce;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;

namespace BookingLibrary.Token
{
    public class TokenJWT
    {
        public string Token { get; private set; } = default!;
        public string Type { get; private set; } = default!;
        public string Full { get { return Type + Token; } }


        public TokenJWT(HttpRequest request)
        {
            SetTokenJWT(request);
        }


        public static TokenJWT GetTokenJWT(HttpRequest request) 
        {
            TokenJWT tokenJWT = new TokenJWT(request);
            return tokenJWT;
        }

        protected void SetTokenJWT ( HttpRequest request )
        {
            var authHeader = request.Headers["Authorization"].ToString();

            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                throw ResponseFormat.JWT_INVALID.Exception;
            }
            SetToken(authHeader.Trim());
        }

        private void SetToken( string fullToken )
        {
            string[] parts = fullToken.Split(new[] { ' ' }, 2);
            Type = parts[0];
            Token = parts.Length > 1 ? parts[1] : string.Empty;
            if (String.IsNullOrEmpty(Type) || String.IsNullOrEmpty(Token))
            {
                throw ResponseFormat.JWT_INVALID.Exception;
            }
        }

        private string tokenId = "";
        public string TokenId
        {
            get
            {
                if (!string.IsNullOrEmpty(tokenId)) return tokenId;
                return GetTokenIDByJWT(Token);
            }
        }

        public static string GetTokenIDByJWT( string token )
        {
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            JwtSecurityToken jwtToken = handler.ReadJwtToken(token);
            string? tokenIdTemp = jwtToken.Claims.FirstOrDefault(c => c.Type == "token_id")?.Value;
            if (tokenIdTemp == null) throw ResponseFormat.TOKEN_ID_INVALID.Exception;
            string tokenId = tokenIdTemp;
            return tokenId;
        }
    }
}
