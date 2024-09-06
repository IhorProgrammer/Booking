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
        private readonly ILoggerManager _loggerManager;
        private readonly IMessageSender _messageSender;
        private static readonly HttpClient client = new HttpClient();



        public ClientController(ClientDBContext context, ILoggerManager loggerManager, IMapper mapper, IConfiguration configuration, IMessageSender messageSender, TokenServer tokenService)
        {
            _context = context;
            _hashService = HashHelper.Type(HashType.MD5);
            _loggerManager = loggerManager;
            _mapper = mapper;
            _configuration = configuration;
            _messageSender = messageSender;
            _tokenService = tokenService;
        }


        //{data: {login:"",password: ""}, user_agent: ""}
        [HttpGet("{data}/{iv}")]
        public async Task<object> Get(string data, string iv)
        {
            string jwt = TokenJWT.GetTokenJWT(Request).Token;
            EncryptionDecryptionModel<AuthModel> decryptionAuthModel = _tokenService.Decryption<AuthModel>(data, iv, jwt);

            ClientDAO clientDAO = await ClientDBContext.AuthAsync(decryptionAuthModel.Data.Login, decryptionAuthModel.Data.Password, _context);
            if(!_tokenService.Subscription(decryptionAuthModel.UserAgent, clientDAO.ClientID, clientDAO.Email, jwt))
            {
                return ResponseFormat.TOKEN_SERVER_CONNECTION_NULL.Responce;
            };
            ClientDTO clientDTO = _mapper.Map<ClientDTO>(clientDAO);
            return ResponseFormat.GetResponceJson( ResponseFormat.AUTHORIZATION_VALID, clientDTO);

        }

        [HttpPost]
        public async Task<object> Post()
        {
            string? userAgent = Request.Form["user_agent"];
            if (userAgent == null) return ResponseFormat.USER_AGENT_EMPTY.Responce;
            //получение DTO 
            string jwt = TokenJWT.GetTokenJWT(Request).Token;
            ClientDTO client = await ClientDTO.ToClientModel(Request.Form, Request.Form.Files);

            ClientDAO clientDAO = _mapper.Map<ClientDAO>(client);
            clientDAO.ClientID = Guid.NewGuid().ToString();
            clientDAO.Salt = _hashService.HashString(Guid.NewGuid().ToString());
            clientDAO.DerivedKey = _hashService.HashString(clientDAO.Salt + client.Password);
            _context.ClientData.Add(clientDAO);
            await _context.SaveChangesAsync();

            //подписка на токен
            if (!_tokenService.Subscription(userAgent, clientDAO.ClientID, clientDAO.Email, jwt))
            {
                return ResponseFormat.TOKEN_SERVER_CONNECTION_NULL.Responce;
            };
            ClientDTO clientDTO = _mapper.Map<ClientDTO>(clientDAO);
            return ResponseFormat.GetResponceJson(ResponseFormat.REGISTRATION_VALID, clientDTO);
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
