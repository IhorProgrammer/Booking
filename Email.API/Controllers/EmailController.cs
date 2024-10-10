using Microsoft.AspNetCore.Mvc;
using Email.API.Services.Email;
using Email.API.Models;
using Email.API.Data;
using Microsoft.Extensions.Options;
using BookingLibrary.Data.DAO;
using Email.API.ResponseFormatExtended;
using BookingLibrary.Token;

namespace Email.API.Controllers
{
    [Route("")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IEmail _email;
        private readonly IConfiguration _configuration;
        private readonly EmailDBContext _context;


        public string EmailFromPassword => _configuration["AppSettings:EmailFromPassword"] ?? throw ResponseFormat.APP_SETTINGS_EmailFromPassword.Exception;
        public string EmailFrom => _configuration["AppSettings:EmailFrom"] ?? throw ResponseFormat.APP_SETTINGS_EmailFrom.Exception;
        public string FromName => _configuration["AppSettings:FromName"] ?? throw ResponseFormat.APP_SETTINGS_FromName.Exception;


        public EmailController(IEmail email, IConfiguration configuration, EmailDBContext context)
        {
            _email = email;
            _configuration = configuration;
            _context = context;
        }

        [HttpGet("{type}")]
        public async Task<object> Get(string type)
        {
            string token_id = TokenJWT.GetTokenJWT(Request).TokenId;

            TokenInfoDAO? tokenInfoData = await _context.TokenInfoData.FindAsync(token_id);
            if (tokenInfoData == null) throw new ArgumentNullException("Token emty: " + token_id, nameof(token_id));
            UserInfoDAO? userInfoData = await _context.UserInfoData.FindAsync(tokenInfoData.UserID);
            if (userInfoData == null)
            {
                throw ResponseFormat.EMAIL_USER_DONT_FIND.ExceptionF(new Exception($"UserInfoData null. TokenId: {token_id}"));
            }

            string? email = userInfoData.Email;
            if (email == null)
            {
                throw ResponseFormat.EMAIL_EMAIL_DONT_FIND.ExceptionF(new Exception($"UserInfoData null. TokenId: {token_id}")).LogLevelSet(LogLevel.Error);
            }
            
            EmailModel emailModel = new EmailModel()
            {
                From = EmailFrom,
                FromPassword = EmailFromPassword,
                FromName = FromName,
                To = email,
                ToName = "Client",
            };
            try
            {
                bool res = new MessageSender.MessageSender(_email).SendByType(emailModel, type);
                return res;
            }
            catch (Exception ex)
            {
                throw ResponseFormat.EMAIL_MESSAGE_DONT_SEND.ExceptionF(ex).LogLevelSet(LogLevel.Critical);
            }
        }


        [HttpPost]
        public async Task<object> Post(PostRequestModel model)
        {
            string token_id = model.TokenID;
            if (string.IsNullOrEmpty(token_id)) { token_id = TokenJWT.GetTokenJWT(Request).TokenId; }

            bool userInfoDataCreated = false;
            UserInfoDAO? userInfoData = await _context.UserInfoData.FindAsync(model.UserID);
            if(userInfoData == null)
            {
                userInfoData = new UserInfoDAO() { Email = model.Email, UserID = model.UserID };
                userInfoDataCreated = true;
            }
            TokenInfoDAO tokenInfoData = new TokenInfoDAO() { TokenID = token_id, UserID = model.UserID };

            if(userInfoDataCreated) await _context.UserInfoData.AddAsync(userInfoData);
            await _context.TokenInfoData.AddAsync(tokenInfoData);
            await _context.SaveChangesAsync();

            return ResponseFormat.EMAIL_TOKEN_CREATED.Responce;
        }



        [HttpPut]
        public async Task<object> Put(string email)
        {
            string token_id = TokenJWT.GetTokenJWT(Request).TokenId;

            TokenInfoDAO? tokenInfoData = await _context.TokenInfoData.FindAsync(token_id);
            if (tokenInfoData == null) throw new ArgumentNullException("Token emty: " + token_id, nameof(token_id));
            UserInfoDAO? userInfoData = await _context.UserInfoData.FindAsync(tokenInfoData.UserID);
            if (userInfoData == null)
            {
                throw ResponseFormat.EMAIL_USER_DONT_FIND.ExceptionF(new Exception($"UserInfoData null. TokenId: {token_id}")).LogLevelSet(LogLevel.Error);
            }

            userInfoData.Email = email;
            await _context.SaveChangesAsync();
            
            return ResponseFormat.EMAIL_TOKEN_CHANGED.Responce;
        }

        [HttpDelete]
        public async Task<object> Delete(string token_id)
        {
            TokenInfoDAO? tokenInfoData = await _context.TokenInfoData.FindAsync(token_id);
            if (tokenInfoData != null)
            {
                _context.Remove(tokenInfoData);
                _context.SaveChanges();
            }

            return ResponseFormat.EMAIL_TOKEN_DELETED.Responce;
        }

    }
}
