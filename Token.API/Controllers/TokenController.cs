using AutoMapper;
using BookingLibrary.Data;
using BookingLibrary.Data.DAO;
using BookingLibrary.Data.DTO;
using BookingLibrary.Helpers.Hash;
using BookingLibrary.Helpers.Hash.HashTypes;
using BookingLibrary.Helpers.HttpClientHelperNamespace;
using BookingLibrary.JsonResponce;
using BookingLibrary.Services.LoggerService;
using BookingLibrary.Services.MessageSender;
using BookingLibrary.Services.TokenService.Model;
using BookingLibrary.Token;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Token.API.Contracts;
using Token.API.Models;
using Token.API.Services;
using Token.API.Services.Tokens;

namespace Token.API.Controllers
{
    [Route("")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly TokenDBContext _context;
        private readonly IHash _hashService;
        private readonly ILoggerManager _loggerManager;
        private readonly IMapper _mapper;
        private readonly IMessageSender _messageSender;
        private readonly IConfiguration _configuration;


        public string Secret => _configuration["AppSettings:Secret"] ?? throw ResponseFormat.APP_SETTINGS_SECRET_NULL.Exception;
        public string Issuer => _configuration["AppSettings:Issuer"] ?? throw ResponseFormat.APP_SETTINGS_ISSUER_NULL.Exception;
        public string Audience => _configuration["AppSettings:Audience"] ?? throw ResponseFormat.APP_SETTINGS_AUDIENCE_NULL.Exception;


        public TokenController(TokenDBContext context,  ILoggerManager loggerManager, IMapper mapper, IMessageSender messageSender, IConfiguration configuration)
        {
            _context = context;
            _hashService = HashHelper.Type(HashType.SHA256);
            _loggerManager = loggerManager;
            _mapper = mapper;
            _messageSender = messageSender;
            _configuration = configuration;
        }

        [HttpGet("session/{data}/{iv}")]
        public async Task<Object> GetSessions(string data, string iv)
        {
            var tokenServED = TokenServiceED.GetTokenServiceED(_context, Request);
            var decryptionJson = tokenServED.JsonDecrypt(data, iv);
            TokenService.GetTokenService(Request, _context).Check.Check(tokenServED.UserAgentByJsonDecrypt(decryptionJson), Secret, Issuer, Audience);
            EncryptionDecryptionModel<GetSessionRequest>? jsonResponseFormat = JsonSerializer.Deserialize< EncryptionDecryptionModel<GetSessionRequest> >(decryptionJson);
            List<TokenDAO> listTokensDAO = await TokenDBContext.FindTokensAsync(jsonResponseFormat.Data.ClientID, _context);
            List<TokenDTO> listTokensDTO = _mapper.Map<List<TokenDTO>>(listTokensDAO);
            return ResponseFormat.GetResponceJson(ResponseFormat.GET_SESSION_OK, listTokensDTO);
        }

        [HttpDelete("session/{data}/{iv}")]
        public async Task<Object> DeleteSessions(string data, string iv)
        {
            var tokenServED = TokenServiceED.GetTokenServiceED(_context, Request);
            var decryptionJson = tokenServED.JsonDecrypt(data, iv);
            TokenService.GetTokenService(Request, _context).Check.Check(tokenServED.UserAgentByJsonDecrypt(decryptionJson), Secret, Issuer, Audience);
            EncryptionDecryptionModel<DeleteSessionRequest>? encryption = JsonSerializer.Deserialize<EncryptionDecryptionModel<DeleteSessionRequest>>(decryptionJson);

            //EmailDelete

            var delParams = new Dictionary<string, string>
            {
                { "token_id", encryption.Data.TokenID },
            };
            string? connection = _configuration.GetConnectionString("EmailServerConnection");
            if (connection == null)
            {
                _loggerManager.LogError(new ArgumentNullException("EmailServerConnection null", nameof(connection)));
                return ResponseFormat.SERVER_ERROR.Responce;
            }
            var tokenResult = await HttpClientHelper.SendDeleteRequest<JsonResponseFormat<object>>(connection, delParams, tokenServED.Full);

            if (await TokenDBContext.DeleteTokenAsync(encryption.Data.TokenID, _context) == true)
            {
                return ResponseFormat.GetResponceJson(ResponseFormat.REMOVE_SESSION_OK, true);
            }
            return ResponseFormat.GetResponceJson(ResponseFormat.REMOVE_SESSION_FAIL, false);
        }

        [HttpGet("token-check/{userAgent}")]
        public async Task<Object> Get(string userAgent)
        {
            var tokenCheckRequest = TokenService.GetTokenService(Request, _context).Check.Check(userAgent, Secret, Issuer, Audience);
            TokenDAO tokenData = tokenCheckRequest.TokenData;
            if( tokenData.UserID == null ) return ResponseFormat.TOKEN_NOT_SIGNED.Responce;
            return ResponseFormat.GetResponceJson(ResponseFormat.TOKEN_VALID, tokenData.Salt);
        }  

        [HttpGet("decryption/{data}/{iv}")]
        public async Task<Object> Get(string data, string iv)
        {

            var tokenServED = TokenServiceED.GetTokenServiceED(_context, Request);
            var decryptionJson = tokenServED.JsonDecrypt(data, iv);
            TokenService.GetTokenService(Request, _context ).Check.Check(tokenServED.UserAgentByJsonDecrypt(decryptionJson), Secret, Issuer, Audience);
            return ResponseFormat.GetResponceJson(ResponseFormat.DATA_DECODED, decryptionJson);
        }

        [HttpPost]
        public async Task<Object> Post()
        {
            string? userAgent = Request.Form["user_agent"];
            if(  userAgent == null ) return ResponseFormat.USER_AGENT_EMPTY.Responce;
            if (String.IsNullOrEmpty(userAgent)) return ResponseFormat.USER_AGENT_EMPTY.Responce;
            TokenGenerationRequest tokenData = await new TokenGenerationRequest(_context).GetToken(userAgent, Secret, Issuer, Audience);
            return ResponseFormat.GetResponceJson(ResponseFormat.TOKEN_GENERATED, tokenData.TokenGenerationResponse);
        }

        [HttpPut]
        public async Task<Object> Put(PutRequestModel requestModel)
        {

            string? tokenId = requestModel.TokenId;
            TokenJWT tokenJWT = TokenJWT.GetTokenJWT(Request);
            if (tokenId == null)
            {
                tokenId = tokenJWT.TokenId;
                if (tokenId == null) return ResponseFormat.JWT_NOT_FOUND.Responce;
            }

            //find tokenData and update tokenUsed
            await TokenDBContext.Subscribe(tokenId, requestModel.UserId, tokenJWT.Token,  _context);



            using (HttpClient httpClient = new HttpClient())
            {

                var model = new { email = requestModel.Email, id_user = requestModel.UserId, id_token = tokenId };
                var jsonContent = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");

                string? connection = _configuration.GetConnectionString("EmailServerConnection");
                if (connection == null)
                {
                    _loggerManager.LogError(new ArgumentNullException("EmailServerConnection null", nameof(connection)));
                    return ResponseFormat.SERVER_ERROR.Responce;
                }
                HttpResponseMessage response;
                try
                {
                    response = await httpClient.PostAsync(connection, jsonContent);
                }
                catch (Exception e) 
                {
                    await TokenDBContext.UnSubscribe(tokenId, requestModel.UserId, _context);
                    return ResponseFormat.SERVER_ERROR.Responce;
                }

                if (response.IsSuccessStatusCode)
                {
                    return ResponseFormat.TOKEN_EMAIL_SUB.Responce;
                }
                else
                {
                    await TokenDBContext.UnSubscribe(tokenId, requestModel.UserId, _context);
                    return ResponseFormat.SERVER_ERROR.Responce;
                }
            }
        }
    }
}
