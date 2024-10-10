using AutoMapper;
using BookingLibrary.Data.DAO;
using BookingLibrary.Data.DTO;
using BookingLibrary.Helpers.Hash;
using BookingLibrary.Helpers.Hash.HashTypes;
using BookingLibrary.JsonResponce;
using BookingLibrary.Services.LoggerService;
using BookingLibrary.Services.MessageSender;
using BookingLibrary.Services.TokenService;
using BookingLibrary.Services.TokenService.Model;
using BookingLibrary.Token;
using Client.API.Data;
using Client.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Client.API.Controllers
{
    [Route("")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly ClientDBContext _context;
        private readonly TokenServer _tokenService;
        private readonly IHash _hashService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IMessageSender _messageSender;
        private static readonly HttpClient client = new HttpClient();

        public ClientController(ClientDBContext context, IMapper mapper, IConfiguration configuration, IMessageSender messageSender, TokenServer tokenService)
        {
            _context = context;
            _hashService = HashHelper.Type(HashType.MD5);
            _mapper = mapper;
            _configuration = configuration;
            _messageSender = messageSender;
            _tokenService = tokenService;
        }

        [HttpGet]
        public string isWork()
        {
            return "Is work";
        }

        //{data: {login:"",password: ""}, user_agent: ""}
        [HttpGet("{data}/{iv}")]
        public async Task<object> Get(string data, string iv)
        {
            string jwt = TokenJWT.GetTokenJWT(Request).Token;
            EncryptionDecryptionModel<AuthModel> decryptionAuthModel = _tokenService.Decryption<AuthModel>(data, iv, jwt);

            ClientDAO clientDAO = await ClientDBContext.AuthAsync(decryptionAuthModel.Data.Login, decryptionAuthModel.Data.Password, _context);

            if (!_tokenService.Subscription(decryptionAuthModel.UserAgent, clientDAO.ClientID, clientDAO.Email, jwt))
            {
                return ResponseFormat.TOKEN_SERVER_CONNECTION_NULL.Responce;
            };

            ClientDTO clientDTO = _mapper.Map<ClientDTO>(clientDAO);
            return ResponseFormat.GetResponceJson(ResponseFormat.AUTHORIZATION_VALID(jwt).SendEmail(_messageSender), clientDTO);

        }

        [HttpGet("image/{url}")]
        public async Task<object> GetImage(string url)
        {
            //Якщо url включає https то звернення йде через нього 
            string urlDecode = WebUtility.UrlDecode(url);
            if ( urlDecode.StartsWith("https://", StringComparison.OrdinalIgnoreCase) )
            {
                var imageBytes = await client.GetByteArrayAsync(urlDecode);
                return File(imageBytes, "image/jpeg");
            }

            //Якщо ні то це наше фото
            // Якщо URL не включає "https", вважаємо, що це шлях до локального файлу
            string filePath = Path.Combine("image", urlDecode); // або інший шлях до ваших файлів

            if (System.IO.File.Exists(filePath))
            {
                var imageBytes = await System.IO.File.ReadAllBytesAsync(filePath);
                return File(imageBytes, "image/jpeg"); // Повертаємо зображення
            }
            else
            {
                return NotFound("Зображення не знайдено.");
            }
        }

        [HttpPost]
        public async Task<object> Post()
        {
            ClientDTO client = await ClientDTO.ToClientModel(Request.Form, Request.Form.Files);

            if (client.GmailID != null)
            {
                ClientDAO? clientDAOGmail = null;
                try
                {
                    clientDAOGmail = await ClientDBContext.AuthEmailAsync(client.Email, client.Password, _context);
                    ClientDTO clientDTOGmail = _mapper.Map<ClientDTO>(clientDAOGmail);
                    return ResponseFormat.GetResponceJson(ResponseFormat.AUTHORIZATION_GOOGLE_VALID, clientDTOGmail);
                }
                catch (JsonResponseFormatException ex)
                {
                    //AuthError
                    clientDAOGmail = _mapper.Map<ClientDAO>(client);
                    clientDAOGmail.ClientID = Guid.NewGuid().ToString();
                    clientDAOGmail.Salt = _hashService.HashString(Guid.NewGuid().ToString());
                    clientDAOGmail.DerivedKey = _hashService.HashString(clientDAOGmail.Salt + client.Password);
                    _context.ClientData.Add(clientDAOGmail);
                    await _context.SaveChangesAsync();
                    ClientDTO clientDTOGmail = _mapper.Map<ClientDTO>(clientDAOGmail);
                    return ResponseFormat.GetResponceJson(ResponseFormat.REGISTRATION_GOOGLE_VALID, clientDTOGmail);
                }
            }

            string jwt = TokenJWT.GetTokenJWT(Request).Token;
            string? userAgent = Request.Form["user_agent"];
            if (userAgent == null) return ResponseFormat.USER_AGENT_EMPTY.Responce;

            ClientDAO clientDAO = _mapper.Map<ClientDAO>(client);
            clientDAO.ClientID = Guid.NewGuid().ToString();
            clientDAO.Salt = _hashService.HashString(Guid.NewGuid().ToString());
            clientDAO.DerivedKey = _hashService.HashString(clientDAO.Salt + client.Password);
            _context.ClientData.Add(clientDAO);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            //подписка на токен
            if (!_tokenService.Subscription(userAgent, clientDAO.ClientID, clientDAO.Email, jwt))
            {
                return ResponseFormat.TOKEN_SERVER_CONNECTION_NULL.Responce;
            };
            ClientDTO clientDTO = _mapper.Map<ClientDTO>(clientDAO);
            return ResponseFormat.GetResponceJson(ResponseFormat.REGISTRATION_VALID, clientDTO);
        }

        [HttpPost("/change")]
        public async Task<object> PostChange()
        {
            string jwt = TokenJWT.GetTokenJWT(Request).Token;
            string? userAgent = Request.Form["user_agent"];
            if (userAgent == null) return ResponseFormat.USER_AGENT_EMPTY.Responce; 
            ClientDTO client = await ClientDTO.ToClientModel(Request.Form, Request.Form.Files);
            //123

            ClientDAO? clientDAO = await ClientDBContext.AuthAsync(client.Nickname, client.Password, _context);
            
            // Перевірка що токен є токеном користувача 

            if (clientDAO == null) throw ResponseFormat.CHANGE_CLIENT_INFO_INVALID.Exception;
            clientDAO.ChangeData(client);
            ClientDAO? isFind = await _context.ClientData.FirstOrDefaultAsync(x => x.Nickname.Equals(client.NewNickname));
            if (isFind != null) return ResponseFormat.CHANGE_NICKNAME_EXISTS.Responce;
            await _context.SaveChangesAsync();

            ClientDTO clientDTO = _mapper.Map<ClientDTO>(clientDAO);
            return ResponseFormat.GetResponceJson(ResponseFormat.CHANGE_CLIENT_INFO, clientDTO);
        }

        //[HttpDelete]
        //public async Task<object> Delete(string login, string password)
        //{
        //    throw new Exception("Email send to Sub Token");


        //    ClientData? clientData = await _context.ClientData.FirstOrDefaultAsync(user => user.Nickname.Equals(login));
        //    if (clientData == null)
        //    {
        //        return ResponseFormat.BadRequest("Login or password invalid");
        //    }
        //    string dk = _hashService.HexString(clientData.Salt + password);
        //    if (!dk.Equals(clientData.DerivedKey))
        //    {
        //        return ResponseFormat.BadRequest("Login or password invalid");
        //    }

        //    if (clientData.ClientPasportData != null)
        //    {
        //        _context.ClientPasportData.Remove(clientData.ClientPasportData);
        //        clientData.PasportID = null;
        //    }

        //    //Зделать неактивным все даные (недвижимость, компании)


        //    throw new Exception("Error");
        //}
    }
}
