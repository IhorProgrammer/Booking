using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ProjectLibrary.Data;
using ProjectLibrary.Data.Entities;
using ProjectLibrary.Models;
using ProjectLibrary.Models.DTO;
using ProjectLibrary.Models.EncryptionDecryptionModel;
using ProjectLibrary.Services.AES;
using ProjectLibrary.Services.Hash;
using ProjectLibrary.Services.JsonResponce;
using ProjectLibrary.Services.LoggerService;
using System.Collections;
using System.Net;
using System.Net.Mime;
using System.Security.Claims;
using System.Text.Json;
using Token.API.Contracts;
using Token.API.Models;

namespace Token.API.Controllers
{
    [Route("")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly DBContext _context;
        private readonly IHashService _hashService;
        private readonly ILoggerManager _loggerManager;
        private readonly IMapper _mapper;
        private readonly AppSettings _appSettings;

        private const int TokenExpires = 86400;


        public TokenController(DBContext context, IHashService hashService, ILoggerManager loggerManager, IMapper mapper, IOptions<AppSettings> appSettings)
        {
            _context = context;
            _hashService = hashService;
            _loggerManager = loggerManager;
            _mapper = mapper;
            _appSettings = appSettings.Value;
        }

        [HttpGet("{userAgent}")]
        public async Task<Object> Get(string userAgent)
        {
            var authHeader = Request.Headers["Authorization"].ToString();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return ResponceFormat.Unauthorized("token empty");
            }
            string? token = authHeader.Substring("Bearer ".Length).Trim();
            
            TokenCheckRequest tokenCheckRequest = new TokenCheckRequest(_context, _appSettings);
            ClaimsPrincipal? principal = await tokenCheckRequest.Check(token, userAgent);

            Claim? tokenIdClaim = principal?.Claims.FirstOrDefault(c => c.Type == "token_id");
            string? tokenId = tokenIdClaim?.Value;
            if (tokenId == null)
            {
                //send email несанкиционированый доступ
                throw new InvalidOperationException("Email Service");
            }
            TokenData? tokenData = await tokenCheckRequest.GetTokenData(tokenId);
            if (tokenData == null) throw new ArgumentNullException("TokenData null");
            if( tokenData.UserID == null ) return ResponceFormat.Unauthorized("token validation");
            return ResponceFormat.OK("token validation", tokenData.Salt);
        }
        

        //Розшифровка тексту і перевірка userAgent на коректність
        [HttpGet("decryption/{data}/{iv}")]
        public async Task<Object> Get(string data, string iv)
        {
            var authHeader = Request.Headers["Authorization"].ToString();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return ResponceFormat.Unauthorized("token empty");
            }
            string? token = authHeader.Substring("Bearer ".Length).Trim();

            //отримання токену 
            TokenCheckRequest tokenCheckRequest = new TokenCheckRequest(_context, _appSettings);
            string? salt = tokenCheckRequest.GetSalt(token);
            if (salt == null) throw new ArgumentNullException($"Salt null ${salt}");
            TokenData? tokenData = await tokenCheckRequest.GetTokenData(salt);
            if(tokenData == null) throw new ArgumentNullException($"tokenData null. Salt:{salt}");
            //розшифрування тексту

            
            string textJson = AES.Decrypt(Convert.FromBase64String(Uri.UnescapeDataString(data)), Convert.FromBase64String(Uri.UnescapeDataString(iv)), tokenData.Salt);
            DecryptionUserAgentModel? decryptionModel = JsonSerializer.Deserialize<DecryptionUserAgentModel>(textJson);
            if (decryptionModel == null) throw new ArgumentNullException("DecryptionModel null");
            //перевірка userAgent
            ClaimsPrincipal? principal = await tokenCheckRequest.Check(token, decryptionModel.UserAgent);
            if(principal == null) throw new ArgumentNullException("UserAgent or token invalid");


            return ResponceFormat.OK("data decoded", textJson);
        }

        [HttpGet("encryption/{data}/{token}")]
        public Object GetSecret(string data, string token)
        {
            var res = AES.Encrypt(data, token);
            string iv = Convert.ToBase64String(res.iv);
            string encrypted = Convert.ToBase64String(res.encrypted);

            var resN = new EncryptionResponceModel() { IV = Uri.EscapeDataString(iv), Encrypted = Uri.EscapeDataString(encrypted) };
            return ResponceFormat.OK<EncryptionResponceModel>("data encoded", resN);
        }


        [HttpPost]
        public async Task<Object> Post(String userAgent)
        {
            if (String.IsNullOrEmpty(userAgent)) return ResponceFormat.BadRequest("user agent empty");
            if (_appSettings == null) throw new ArgumentNullException("_appSettings null");
            TokenGenerationRequest tokenG = new TokenGenerationRequest(_context, _appSettings);
            TokenGenerationResponce res = await tokenG.GetToken(userAgent);
            return ResponceFormat.OK("token genereted", res);
        }

        [HttpPut]
        public async Task<Object> Put([FromBody] PutRequestModel requestModel)
        {
            //Get jwt token
            var authHeader = Request.Headers["Authorization"].ToString();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return ResponceFormat.Unauthorized("token empty");
            }
            string? token = authHeader.Substring("Bearer ".Length).Trim();
            //Check token
            TokenCheckRequest tokenCheckRequest = new TokenCheckRequest(_context, _appSettings);
            //Get token_id
            ClaimsPrincipal? principal = await tokenCheckRequest.Check(token, requestModel.UserAgent);
            if (principal == null) return ResponceFormat.Unauthorized("token invalid");
            Claim? tokenIdClaim = principal?.Claims.FirstOrDefault(c => c.Type == "token_id");
            string? tokenId = tokenIdClaim?.Value;
            if (tokenId == null) throw new ArgumentNullException($"tokenId empty ${tokenId}");
            //find tokenData and update tokenUsed
            TokenData? tokenData = await tokenCheckRequest.GetTokenData(tokenId);
            if (tokenData == null) throw new ArgumentNullException($"TokenData null ${tokenId}");
            if (tokenData.UserID != null)
            {
                throw new InvalidOperationException("Send Email");
                //return ResponceFormat.Unauthorized("User isn't empty");
            }

            tokenData.UserID = requestModel.UserId;
            tokenData.TokenUsed = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            
            return ResponceFormat.OK("token subscribe", "true");
        }

        //[HttpDelete]
        //public async Task<Object> Delete(String tokenId, String userId)
        //{
        //    if (String.IsNullOrEmpty(userId)) return ResponceFormat.BadRequest("bad request");
        //    TokenData? tokenData = _context.TokensData.FirstOrDefault(t => t.UserID == userId && t.ID == tokenId);
        //    if (tokenData == null) return ResponceFormat.BadRequest("bad request");
        //    _context.TokensData.Remove(tokenData);
        //    await _context.SaveChangesAsync();
        //    return ResponceFormat.OK("token deleted", true);
        //}
    }
}
