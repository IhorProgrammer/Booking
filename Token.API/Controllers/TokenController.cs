using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ProjectLibrary.Data;
using ProjectLibrary.Data.Entities;
using ProjectLibrary.Models;
using ProjectLibrary.Models.DTO;
using ProjectLibrary.Services;
using ProjectLibrary.Services.Hash;
using ProjectLibrary.Services.LoggerService;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
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

        [HttpGet]
        public async Task<String> Get(string userAgent)
        {
            var authHeader = Request.Headers["Authorization"].ToString();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return JsonResponceFormat<String>.GetResponce(HttpStatusCode.Unauthorized, "token empty", "");
            }
            var token = authHeader.Substring("Bearer ".Length).Trim();

            TokenCheckRequest tokenCheckRequest = new TokenCheckRequest(_context, _appSettings);
            ClaimsPrincipal? principal = await tokenCheckRequest.Check(token, userAgent);

            Claim? saltClaim = principal?.Claims.FirstOrDefault(c => c.Type == "salt_id");
            string? salt = saltClaim?.Value;
            if (salt == null)
            {
                //send email несанкиционированый доступ
                throw new InvalidOperationException("Email Service");
            }
            TokenData? tokenData = await tokenCheckRequest.GetTokenData(salt);
            if (tokenData == null) throw new ArgumentNullException("TokenData null");
            if( tokenData.UserID == null ) return JsonResponceFormat<String>.GetResponce(HttpStatusCode.Unauthorized, "token validation", "");
            tokenData.TokenUsed = DateTime.Now;
            await _context.SaveChangesAsync();
            return JsonResponceFormat<String>.GetResponce(HttpStatusCode.OK, "token validation", tokenData.Token );
        }

        [HttpPost]
        public async Task<String> Post(String userAgent)
        {
            if (String.IsNullOrEmpty(userAgent)) return JsonResponceFormat<String>.GetResponce(HttpStatusCode.BadRequest, "user agent empty", "");
            if (_appSettings == null) throw new ArgumentNullException("_appSettings null");
            var tokenG = new TokenGenerationRequest(_context, _appSettings);
            var res = await tokenG.GetToken(userAgent);
            return JsonResponceFormat<TokenGenerationResponce>.GetResponce(HttpStatusCode.OK, "token genereted", res);
        }

        [HttpPut]
        public async Task<String> Put(String userAgent, string userId)
        {
            var authHeader = Request.Headers["Authorization"].ToString();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return JsonResponceFormat<String>.GetResponce(HttpStatusCode.Unauthorized, "token empty", "");
            }
            var token = authHeader.Substring("Bearer ".Length).Trim();

            TokenCheckRequest tokenCheckRequest = new TokenCheckRequest(_context, _appSettings);
            ClaimsPrincipal? principal = await tokenCheckRequest.Check(token, userAgent);
            if(principal == null) return JsonResponceFormat<String>.GetResponce(HttpStatusCode.Unauthorized, "token invalid", "");
            Claim? saltClaim = principal?.Claims.FirstOrDefault(c => c.Type == "salt_id");
            string? salt = saltClaim?.Value;
            if (salt == null) throw new ArgumentNullException($"salt empty ${salt}");
            TokenData? tokenData = await tokenCheckRequest.GetTokenData(salt);
            if (tokenData == null) throw new ArgumentNullException($"TokenData null ${salt}");
            if (tokenData.UserID != null) return JsonResponceFormat<String>.GetResponce(HttpStatusCode.Unauthorized, "User isn't empty", "");
            tokenData.UserID = userId;
            await _context.SaveChangesAsync();

            return JsonResponceFormat<Boolean>.GetResponce(HttpStatusCode.OK, "token subscribe", true);
        }

        [HttpDelete]
        public async Task<String> Delete(int tokenId, String userId)
        {
            if (String.IsNullOrEmpty(userId)) return JsonResponceFormat<String>.GetResponce(HttpStatusCode.BadRequest, "bad request", "");
            TokenData? tokenData = _context.TokensData.FirstOrDefault(t => t.UserID == userId && t.ID == tokenId);
            if(tokenData == null) return JsonResponceFormat<String>.GetResponce(HttpStatusCode.BadRequest, "bad request", "");
            _context.TokensData.Remove(tokenData);
            await _context.SaveChangesAsync();
            return JsonResponceFormat<Boolean>.GetResponce(HttpStatusCode.OK, "token deleted", true);
        }
    }
}
