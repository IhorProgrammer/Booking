using AutoMapper;
using Client.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using ProjectLibrary.Data;
using ProjectLibrary.Data.Entities;
using ProjectLibrary.Models.DTO;
using ProjectLibrary.Models.EncryptionDecryptionModel;
using ProjectLibrary.Services.AES;
using ProjectLibrary.Services.Hash;
using ProjectLibrary.Services.JsonResponce;
using ProjectLibrary.Services.LoggerService;
using ProjectLibrary.Services.TokenService;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Client.API.Controllers
{
    [Route("")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly DBContext _context;
        private readonly IHashService _hashService;
        private readonly ILoggerManager _loggerManager;
        private readonly IMapper _mapper;
        private static readonly HttpClient client = new HttpClient();
        private readonly IConfiguration _configuration;

        public ClientController(DBContext context, IHashService hashService, ILoggerManager loggerManager, IMapper mapper, IConfiguration configuration)
        {
            _context = context;
            _hashService = hashService;
            _loggerManager = loggerManager;
            _mapper = mapper;
            _configuration = configuration;
        }


        /*
         * Auth user
         * {data: {login:"",password: ""}, user_agent: ""}
        */
        [HttpGet("{data}/{iv}")]
        public async Task<object> Get(string data, string iv) {
            Request.HttpContext.Response.ContentType = "application/json";

            //Check token
            TokenService tokenService = new TokenService(_configuration);
            EncryptionDecryptionModel<AuthModel>? decryptionAuthModel = tokenService.DecryptionData<AuthModel>(Request, data, iv); ;
            if (decryptionAuthModel == null) return ResponceFormat.BadRequest("token invalid");

            //Get clientData
            ClientData? clientData = await _context.ClientData.FirstOrDefaultAsync( user => user.Nickname.Equals(decryptionAuthModel.Data.Login) );
            if( clientData == null ) return ResponceFormat.BadRequest("Login or password invalid");
            bool isValidate = clientData.ValidatePassword(decryptionAuthModel.Data.Password, _hashService);
            if(!isValidate) return ResponceFormat.BadRequest("Login or password invalid");


            //Token Subscription
            if (!tokenService.TokenSubscription(Request, decryptionAuthModel.UserAgent, clientData.ClientID))
                return ResponceFormat.BadRequest("Login or password invalid");

            ClientModel clientModel = _mapper.Map<ClientModel>(clientData);
            return ResponceFormat.OK<ClientModel>("User information", clientModel);
        }


        [HttpPost]
        public async Task<object> Post(ClientModel client)
        {
            if (!ModelState.IsValid)
            {
                return ResponceFormat.BadRequest("bad request");
            }

            if (!client.isValid())
            {
                return ResponceFormat.BadRequest("validate error");
            }

            var clientData = _mapper.Map<ClientData>(client);
            clientData.ClientID = Guid.NewGuid().ToString();
            clientData.Salt = _hashService.HexString(Guid.NewGuid().ToString());
            clientData.DerivedKey = _hashService.HexString(clientData.Salt + client.Password);

            // Додавання нового клієнта
            _context.ClientData.Add(clientData);

            await _context.SaveChangesAsync();

            return ResponceFormat.Created("user created", clientData.ClientID);
        }

        [HttpDelete]
        public async Task<object> Delete(string login, string password)
        {
            ClientData? clientData = await _context.ClientData.FirstOrDefaultAsync(user => user.Nickname.Equals(login));
            if (clientData == null)
            {
                return ResponceFormat.BadRequest("Login or password invalid");
            }
            string dk = _hashService.HexString(clientData.Salt + password);
            if (!dk.Equals(clientData.DerivedKey))
            {
                return ResponceFormat.BadRequest("Login or password invalid");
            }

            if (clientData.ClientPasportData != null)
            {
                _context.ClientPasportData.Remove(clientData.ClientPasportData);
                clientData.PasportID = null;
            }

            //Зделать неактивным все даные (недвижимость, компании)


            throw new Exception("Error");
        }
    }
}
