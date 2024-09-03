using BookingLibrary.Data;
using Token.API.Contracts;

namespace Token.API.Services.Tokens
{
    public class TokenJWTCheck
    {
        public string UserAgent { get; private set; } = default!;
  

        public TokenDBContext Context { get; private set; }
        public string Token { get; private set; } = default!;



        public TokenJWTCheck(TokenDBContext context, String token)
        {
            Context = context;
            this.Token = token;
        }

        public TokenCheckRequest Check(string userAgent, string secret, string issuer, string audience)
        {
            UserAgent = userAgent;
            TokenCheckRequest tokenCheckRequest = new TokenCheckRequest(Context).Check(Token, UserAgent, secret, issuer, audience);
            return tokenCheckRequest;
        }

        
    }
}
