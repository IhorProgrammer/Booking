using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Net;
using Email.API.Services.Email;
using Email.API.Models;
using Microsoft.Extensions.Configuration;
using ProjectLibrary.Services.TokenService;
using Email.API.Data;
using Microsoft.EntityFrameworkCore;
using Email.API.Data.Entities;
using ProjectLibrary.Services.LoggerService;
using ProjectLibrary.Models;
using Microsoft.Extensions.Options;
using Email.API.MessageSender;

namespace Email.API.Controllers
{
    [Route("")]
    [ApiController]
    public class EmailController : ControllerBase
    {

        private readonly IEmail _email;
        private readonly IConfiguration _configuration;
        private readonly DBContext _context;
        private readonly ILoggerManager _loggerManager;
        private readonly AppSettings _appSettings;

        public EmailController(IEmail email, IConfiguration configuration, DBContext context, ILoggerManager loggerManager, IOptions<AppSettings> appSettings)
        {
            _email = email;
            _configuration = configuration;
            _context = context;
            _loggerManager = loggerManager;
            _appSettings = appSettings.Value;

            if (String.IsNullOrEmpty(_appSettings.EmailFromPassword)) _loggerManager.LogError(new ArgumentNullException(nameof(_appSettings.EmailFromPassword)));
            if (String.IsNullOrEmpty(_appSettings.EmailFrom)) _loggerManager.LogError(new ArgumentNullException(nameof(_appSettings.EmailFrom)));
            if (String.IsNullOrEmpty(_appSettings.FromName)) _loggerManager.LogError(new ArgumentNullException(nameof(_appSettings.FromName)));

        }

        [HttpGet("{type}")]
        public async Task<bool> Get(string type)
        {
            TokenService tokenService = new TokenService(_configuration);
            string? token = tokenService.GetJWTTokenByRequest(Request);
            if (token == null)
            {
                throw new ArgumentNullException("Token null");
            }
            string? token_id = tokenService.GetTokenId(token);
            if (String.IsNullOrEmpty(token_id)) throw new ArgumentNullException("Salt empty", nameof(token_id));


            TokenInfoData? tokenInfoData = await _context.TokenInfoData.FindAsync(token_id);
            if (tokenInfoData == null) throw new ArgumentNullException("Token emty: " + token_id, nameof(token_id));
            UserInfoData? userInfoData = await _context.UserInfoData.FindAsync(tokenInfoData.);
            if (userInfoData == null)
            {
                _loggerManager.LogError(new Exception($"UserInfoData null. TokenId: {token_id}"));
                return false;
            }

            string? email = userInfoData.Email;
            if (email == null) _loggerManager.LogError(new Exception($"UserInfoData null. TokenId: {token_id}"));


            if (_appSettings.EmailFrom == null || _appSettings.EmailFromPassword == null || _appSettings.FromName == null || email == null)
            {
                _loggerManager.LogError(new Exception("_appSettings null"));
                return false;
            } 
            EmailModel emailModel = new EmailModel()
            {
                From = _appSettings.EmailFrom,
                FromPassword = _appSettings.EmailFromPassword,
                FromName = _appSettings.FromName,
                To = email,
                ToName = "Client",
            };
            try
            {
                bool res = new MessageSender.MessageSender(_email).Unauthorized(emailModel);
                return res;
            }
            catch (Exception ex)
            {
                _loggerManager.LogWarning(ex);
            }

            return false;
        }



    }
}
