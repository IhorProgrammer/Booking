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
using ProjectLibrary.Services.MessageSender;
using ProjectLibrary.Services.TokenService;
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
        private readonly DBContext _context;
        private readonly IHashService _hashService;
        private readonly ILoggerManager _loggerManager;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IMessageSender _messageSender;
        private static readonly HttpClient client = new HttpClient();

        public ClientController(DBContext context, IHashService hashService, ILoggerManager loggerManager, IMapper mapper, IConfiguration configuration, IMessageSender messageSender)
        {
            _context = context;
            _hashService = hashService;
            _loggerManager = loggerManager;
            _mapper = mapper;
            _configuration = configuration;
            _messageSender = messageSender;
        }


        //{data: {login:"",password: ""}, user_agent: ""}
        [HttpGet("{data}/{iv}")]
        public async Task<object> Get(string data, string iv)
        {
            //Check token
            TokenService tokenService = new TokenService(_configuration);
            EncryptionDecryptionModel<AuthModel>? decryptionAuthModel = tokenService.DecryptionData<AuthModel>(Request, data, iv); ;
            if (decryptionAuthModel == null) return ResponseFormat.BadRequest("token invalid");

            //Get clientData
            ClientData? clientData = await _context.ClientData.FirstOrDefaultAsync(user => user.Nickname.Equals(decryptionAuthModel.Data.Login));
            if (clientData == null) return ResponseFormat.BadRequest("Login or password invalid");
            bool isValidate = clientData.ValidatePassword(decryptionAuthModel.Data.Password, _hashService);
            if (!isValidate) return ResponseFormat.BadRequest("Login or password invalid");



            //Token Subscription
            if (!tokenService.TokenSubscription(Request, decryptionAuthModel.UserAgent, clientData.ClientID))
            {
                return ResponseFormat.BadRequest("Login or password invalid");
            }

            string? token = tokenService.GetJWTTokenByRequest(Request);
            _messageSender.Send(token ?? "", MessageSenderTypes.Authorization);
            ClientModel clientModel = _mapper.Map<ClientModel>(clientData);
            return ResponseFormat.OK<ClientModel>("User information", clientModel);
        }

        [HttpPost("{userAgent}")]
        public async Task<object> Post(string userAgent)
        {
            if (!ModelState.IsValid)
            {
                return ResponseFormat.BadRequest("bad request");
            }
            ClientModel client = await ClientModel.ToClientModel(Request.Form, Request.Form.Files);

            if (!client.isValid())
            {
                //Delete File
                return ResponseFormat.BadRequest("validate error");
            }

            TokenService tokenService = new TokenService(_configuration);
            string? jwt = tokenService.GetJWTTokenByRequest(Request);
            if (jwt == null)
            {
                //Delete File
                return ResponseFormat.BadRequest("token invalid");
            }


            var clientData = _mapper.Map<ClientData>(client);
            clientData.ClientID = Guid.NewGuid().ToString();
            clientData.Salt = _hashService.HexString(Guid.NewGuid().ToString());
            clientData.DerivedKey = _hashService.HexString(clientData.Salt + client.Password);

            // Додавання нового клієнта
            _context.ClientData.Add(clientData);

            await _context.SaveChangesAsync();

            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwt);

                var model = new { email = clientData.Email, id_user = clientData.ClientID };
                var jsonContent = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");

                string? connection = _configuration.GetConnectionString("EmailServerConnection");
                if (connection == null)
                {
                    _loggerManager.LogError(new ArgumentNullException("EmailServerConnection null", nameof(connection)));
                    return ResponseFormat.InternalServerError("Server Error");
                }
                HttpResponseMessage response = await httpClient.PostAsync(connection, jsonContent);

                if (!response.IsSuccessStatusCode)
                {
                    _messageSender.Send(jwt, MessageSenderTypes.Authorization);
                    return ResponseFormat.Created("user created", clientData.ClientID);
                }
                else
                {
                    _messageSender.Send(jwt, MessageSenderTypes.Registration);
                }
            }

            return ResponseFormat.Created("user and email created", clientData.ClientID);
        }

        [HttpDelete]
        public async Task<object> Delete(string login, string password)
        {
            ClientData? clientData = await _context.ClientData.FirstOrDefaultAsync(user => user.Nickname.Equals(login));
            if (clientData == null)
            {
                return ResponseFormat.BadRequest("Login or password invalid");
            }
            string dk = _hashService.HexString(clientData.Salt + password);
            if (!dk.Equals(clientData.DerivedKey))
            {
                return ResponseFormat.BadRequest("Login or password invalid");
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
