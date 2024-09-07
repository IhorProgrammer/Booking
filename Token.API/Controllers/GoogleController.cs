using AutoMapper;
using BookingLibrary.Data;
using BookingLibrary.Data.DTO;
using BookingLibrary.Helpers.Hash;
using BookingLibrary.Helpers.Hash.HashTypes;
using BookingLibrary.Helpers.HttpClientHelperNamespace;
using BookingLibrary.JsonResponce;
using BookingLibrary.Services.LoggerService;
using BookingLibrary.Services.MessageSender;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json;
using Token.API.Contracts;
using Token.API.Models;
using Token.API.Services;

namespace Token.API.Controllers
{
    [Route("google")]
    [ApiController]
    public class GoogleController : Controller
    {
        private readonly TokenDBContext _context;
        private readonly IHash _hashService;
        private readonly ILoggerManager _loggerManager;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IMessageSender _messageSender;

        public string Secret => _configuration["AppSettings:Secret"] ?? throw ResponseFormat.APP_SETTINGS_SECRET_NULL.Exception;
        public string Issuer => _configuration["AppSettings:Issuer"] ?? throw ResponseFormat.APP_SETTINGS_ISSUER_NULL.Exception;
        public string Audience => _configuration["AppSettings:Audience"] ?? throw ResponseFormat.APP_SETTINGS_AUDIENCE_NULL.Exception;



        public GoogleController(TokenDBContext context, ILoggerManager loggerManager, IMapper mapper, IMessageSender messageSender, IConfiguration configuration)
        {
            _context = context;
            _hashService = HashHelper.Type(HashType.SHA256); //Only SHA256
            _loggerManager = loggerManager;
            _mapper = mapper;
            _messageSender = messageSender;
            _configuration = configuration;
        }
        
        private string REDIRECT_URL => _configuration["Authentication:Google:REDIRECT_URL"] ?? throw ResponseFormat.APP_SETTINGS_AUDIENCE_NULL.Exception;
        private string PROFILE_SCOPE => _configuration["Authentication:Google:PROFILE_SCOPE"] ?? throw ResponseFormat.APP_SETTINGS_AUDIENCE_NULL.Exception;
        private string EMAIL_SCOPE => _configuration["Authentication:Google:EMAIL_SCOPE"] ?? throw ResponseFormat.APP_SETTINGS_AUDIENCE_NULL.Exception;
        private string PKCE_SESSION_KEY => _configuration["Authentication:Google:PKCE_SESSION_KEY"] ?? throw ResponseFormat.APP_SETTINGS_AUDIENCE_NULL.Exception;
        private string ENDPOINT_URL => _configuration["Authentication:Google:ENDPOINT_URL"] ?? throw ResponseFormat.APP_SETTINGS_AUDIENCE_NULL.Exception;


        [HttpGet("{userAgent}")]
        public async Task<object> GenerateOAuthGoogle(string userAgent)
        {
            TokenCheckRequest tokenCheckRequest = TokenService.GetTokenService(Request, _context).Check.Check(userAgent, Secret, Issuer, Audience);
            string tokenId = tokenCheckRequest.TokenId;
            string encodedTokenId = Convert.ToBase64String(Encoding.UTF8.GetBytes(tokenId));
            var codeChellange = _hashService.HashString(tokenId);
            string profileScope = GoogleOAuthService.GenerateOAuthRequestUrl($"{PROFILE_SCOPE} {EMAIL_SCOPE}", REDIRECT_URL, codeChellange, encodedTokenId);

            return ResponseFormat.GetResponceJson(ResponseFormat.GOOGLE_URL_REDIRECT, profileScope);
        }


        [HttpGet("code")]
        public async Task<object> GetTokens(string code, string state)
        {

            var decodedBytes = Convert.FromBase64String(state);
            string tokenId = Encoding.UTF8.GetString(decodedBytes);


            GoogleOAuthTokenResult? tokenResult = await GoogleOAuthService.ExchangeCodeOnTokenAsync(code, tokenId, REDIRECT_URL);
            if (tokenResult == null) return ResponseFormat.GOOGLE_URL_REDIRECT_EXCEPTION;
            var tokenHandler = new JwtSecurityTokenHandler();
            JwtSecurityToken jwtToken = tokenHandler.ReadJwtToken(tokenResult.IdToken);

            // Витягнути всі претензії
            var claims = jwtToken.Claims;

            // Вивести конкретні претензії
            string? sub = claims.FirstOrDefault(c => c.Type == "sub")?.Value;
            if (sub == null) throw new Exception("Invalid sub");

            ClientDTO clientModel = new ClientDTO();
            
            string email_verified = claims.FirstOrDefault(c => c.Type == "email_verified")?.Value;
            string email = claims.FirstOrDefault(c => c.Type == "email")?.Value;
            string avatar = claims.FirstOrDefault(c => c.Type == "picture")?.Value;
            string familyName = claims.FirstOrDefault(c => c.Type == "family_name")?.Value;
            string givenName = claims.FirstOrDefault(c => c.Type == "given_name")?.Value;

            clientModel = new ClientDTO();
            clientModel.Avatar = avatar;
            clientModel.FamilyName = familyName ?? null; 
            clientModel.GmailID = sub;
            clientModel.FamilyName = familyName ?? null;
            clientModel.GivenName = givenName ?? null;
            clientModel.Password = sub;
            if (email == null) return ResponseFormat.EMAIL_NULL.Exception;
            clientModel.Email = email;
            clientModel.Nickname = email;
            //Відправляєм на сервер де буде отриимання даних користувача ( токен \ id або ж googleId ) || перегляд даних
            //Post запит на реэстарцыю | авторизація користувача (отримання id)
            throw new Exception("Client ID null");
            clientModel.ClientID = "";


            //Token Sub
            using (HttpClient httpClient = new HttpClient())
            {

                var jsonData = new
                {
                    id = clientModel.ClientID,
                    token_id = tokenId,
                    email = clientModel.Email,
                };

                StringContent jsonContent = new StringContent(JsonSerializer.Serialize(jsonData), Encoding.UTF8, "application/json");
                string? connection = _configuration.GetConnectionString("TokenServerConnection");
                HttpResponseMessage response = await httpClient.PutAsync(connection, jsonContent);
                string? res = await response.Content.ReadAsStringAsync();
                if (res == null) throw new Exception("Response tokenService == null. JsonData:" + jsonData);
            }

            return Redirect(ENDPOINT_URL);
        }

    }
}
