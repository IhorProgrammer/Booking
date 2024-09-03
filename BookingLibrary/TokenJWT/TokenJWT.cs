using BookingLibrary.JsonResponce;
using Microsoft.AspNetCore.Http;


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

    }
}
