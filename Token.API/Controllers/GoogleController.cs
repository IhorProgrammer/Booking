using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using ProjectLibrary.Data;
using ProjectLibrary.Data.Entities;
using ProjectLibrary.Models;
using ProjectLibrary.Models.DTO;
using ProjectLibrary.Services.Hash;
using ProjectLibrary.Services.JsonResponce;
using ProjectLibrary.Services.LoggerService;
using ProjectLibrary.Services.MessageSender;
using ProjectLibrary.Services.TokenService;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Token.API.Contracts;
using Token.API.Helpers;
using Token.API.Services;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Token.API.Controllers
{
    [Route("google")]
    [ApiController]
    public class GoogleController : Controller
    {
        private readonly DBContext _context;
        private readonly IHashService _hashService;
        private readonly ILoggerManager _loggerManager;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly AppSettings _appSettings;
        private readonly IMessageSender _messageSender;

        public GoogleController(DBContext context, IHashService hashService, ILoggerManager loggerManager, IMapper mapper, IOptions<AppSettings> appSettings, IMessageSender messageSender, IConfiguration configuration)
        {
            _context = context;
            _hashService = hashService;
            _loggerManager = loggerManager;
            _mapper = mapper;
            _appSettings = appSettings.Value;
            _messageSender = messageSender;
            _configuration = configuration;
        }


        private const string REDIRECT_URL = "http://localhost:5141/google/code";
        private const string PROFILE_SCOPE = "https://www.googleapis.com/auth/userinfo.profile";
        private const string EMAIL_SCOPE = "https://www.googleapis.com/auth/userinfo.email";
        private const string PKCE_SESSION_KEY = "codeVerifier";

        [HttpGet("{userAgent}")]
        public async Task<object> GenerateOAuthGoogle(string userAgent)
        {
            var authHeader = Request.Headers["Authorization"].ToString();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return ResponseFormat.Unauthorized("token empty");
            }
            string? token = authHeader.Substring("Bearer ".Length).Trim();

            TokenCheckRequest tokenCheckRequest = new TokenCheckRequest(_context, _appSettings);
            ClaimsPrincipal? principal = await tokenCheckRequest.Check(token, Uri.UnescapeDataString(userAgent));

            Claim? tokenIdClaim = principal?.Claims.FirstOrDefault(c => c.Type == "token_id");
            string? tokenId = tokenIdClaim?.Value;
            if (tokenId == null) return ResponseFormat.BadRequest("Token invalid");
            
            string encodedTokenId = Convert.ToBase64String(Encoding.UTF8.GetBytes(tokenId));
            var codeChellange = Sha256Helper.ComputeHash(tokenId);

            string profileScope = GoogleOAuthService.GenerateOAuthRequestUrl($"{PROFILE_SCOPE} {EMAIL_SCOPE}", REDIRECT_URL, codeChellange, encodedTokenId);

            return ResponseFormat.OK("URL google", profileScope);
        }


        [HttpGet("code")]
        public async Task<object> GetTokens(string code, string state)
        {

            var decodedBytes = Convert.FromBase64String(state);
            string tokenId = Encoding.UTF8.GetString(decodedBytes);


            var tokenResult = await GoogleOAuthService.ExchangeCodeOnTokenAsync(code, tokenId, REDIRECT_URL);


            var tokenHandler = new JwtSecurityTokenHandler();
            JwtSecurityToken jwtToken = tokenHandler.ReadJwtToken(tokenResult.IdToken);

            // Витягнути всі претензії
            var claims = jwtToken.Claims;

            // Вивести конкретні претензії
            string sub = claims.FirstOrDefault(c => c.Type == "sub")?.Value;
            if (sub == null) throw new Exception("Invalid sub");
            
            TokenService tokenService = new TokenService(_configuration);
            
            ClientData clientData = await _context.ClientData.FirstOrDefaultAsync( c => sub == c.GmailID ) ;
            if( clientData == null )
            {
                string email_verified = claims.FirstOrDefault(c => c.Type == "email_verified")?.Value;
                string email = claims.FirstOrDefault(c => c.Type == "email")?.Value;
                string avatar = claims.FirstOrDefault(c => c.Type == "picture")?.Value;
                string familyName = claims.FirstOrDefault(c => c.Type == "family_name")?.Value;
                string givenName = claims.FirstOrDefault(c => c.Type == "given_name")?.Value;

                clientData = new ClientData();
                clientData.GmailID = sub;
                clientData.ClientID = Guid.NewGuid().ToString();
                clientData.AvatarName = avatar ?? null;
                clientData.FamilyName = familyName ?? null;
                clientData.GivenName = givenName ?? null;
                clientData.Salt = _hashService.HexString(Guid.NewGuid().ToString());
                clientData.DerivedKey = _hashService.HexString(clientData.Salt + clientData.GmailID);
                if (email == null) return ResponseFormat.InternalServerError("Email not found");
                clientData.Email = email;
                if (Boolean.TryParse(email_verified, out bool verified)) clientData.isVerified = verified;
                else clientData.isVerified = false;

                await _context.ClientData.AddAsync(clientData);
                await _context.SaveChangesAsync();
            }


            //Token sub withaut check ua 
            using (HttpClient httpClient = new HttpClient())
            {

                var jsonData = new
                {
                    id = clientData.ClientID,
                    token_id = tokenId,
                    email = clientData.Email,
                };

                StringContent jsonContent = new StringContent(JsonSerializer.Serialize(jsonData), Encoding.UTF8, "application/json");
                string? connection = _configuration.GetConnectionString("TokenServerConnection");
                HttpResponseMessage response = await httpClient.PutAsync(connection, jsonContent);
                string? res = await response.Content.ReadAsStringAsync();
                if (res == null) throw new Exception("Response tokenService == null. JsonData:" + jsonData);
            }


            ClientModel clientModel = _mapper.Map<ClientModel>(clientData);

            return ResponseFormat.OK("User auth", clientModel);
        }

    }
}
