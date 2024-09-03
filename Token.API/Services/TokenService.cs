using BookingLibrary.Data;
using BookingLibrary.Token;
using ProjectLibrary.Models;
using Token.API.Contracts;
using Token.API.Services.Tokens;

namespace Token.API.Services
{
    public class TokenService : TokenJWT
    {
        public TokenDBContext Context { get; private set; }

        public TokenService(TokenDBContext context, HttpRequest request) : base(request) {
            Context = context;
        }

        public TokenJWTCheck Check { get => new TokenJWTCheck(Context, Token); }
        public TokenGenerationRequest TokenGeneration { get => new TokenGenerationRequest(Context); }


        public static TokenService GetTokenService(HttpRequest request, TokenDBContext context) 
        {
            TokenService tokenService = new(context, request);
            return tokenService;
        }


    }
}
