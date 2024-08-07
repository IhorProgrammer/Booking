using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectLibrary.Data;
using ProjectLibrary.Data.Entities;
using ProjectLibrary.Models.DTO;
using ProjectLibrary.Services;
using ProjectLibrary.Services.Hash;
using ProjectLibrary.Services.LoggerService;
using System.Net;

namespace Token.API.Controllers
{
    [Route("/recovery")]
    [ApiController]
    public class TokenRecoveryController : ControllerBase
    {
        private readonly DBContext _context;
        private readonly IHashService _hashService;
        private readonly ILoggerManager _loggerManager;
        private readonly IMapper _mapper;

        private const int TokenExpires = 3600;


        public TokenRecoveryController(DBContext context, IHashService hashService, ILoggerManager loggerManager, IMapper mapper)
        {
            _context = context;
            _hashService = hashService;
            _loggerManager = loggerManager;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<String> Get(string token)
        {
            var timeNow = DateTime.Now;
            TokenData? tokenData = await _context.TokensData.FirstOrDefaultAsync(
                (token) =>
                token.ID.Equals(token)
                && token.TokenExpires > timeNow
                && token.TokenType == TokenType.Recovery
            );

            if (tokenData == null)
            {
                return JsonResponceFormat<String>.GetResponce("failed", HttpStatusCode.BadRequest, "token is invalid", "");
            }

            tokenData.TokenExpires = timeNow.AddSeconds(TokenExpires);

            await _context.SaveChangesAsync();

            TokenModel tokenModel = _mapper.Map<TokenModel>(tokenData);
            return JsonResponceFormat<TokenModel>.GetResponce("success", HttpStatusCode.OK, "token is valid", tokenModel);
        }

        [HttpPost]
        public async Task<String> Post(String user_id) 
        {
            if( !String.IsNullOrEmpty(user_id) && user_id.Length != 36 )
            {
                return JsonResponceFormat<String>.GetResponce("failed", HttpStatusCode.BadRequest, "user_id is invalid", "");
            }

            ClientData? clientData = _context.ClientData.Find(user_id);
            if (clientData == null) {
                return JsonResponceFormat<String>.GetResponce("failed", HttpStatusCode.BadRequest, "user_id is invalid", "");
            }

            TokenData tokenData = new TokenData();
            tokenData.ID = Guid.NewGuid().ToString();
            tokenData.UserID = user_id;
            tokenData.TokenCreated = DateTime.Now;
            tokenData.TokenExpires = DateTime.Now.AddSeconds(TokenExpires);
            tokenData.TokenType = TokenType.Recovery;

            await _context.TokensData.AddAsync(tokenData);
            await _context.SaveChangesAsync();

            return JsonResponceFormat<String>.GetResponce("success", HttpStatusCode.Created, "Token created", "");
        }

    }
}
