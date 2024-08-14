using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using ProjectLibrary.Data;
using ProjectLibrary.Data.Entities;
using ProjectLibrary.Models.DTO;
using ProjectLibrary.Services;
using ProjectLibrary.Services.Hash;
using ProjectLibrary.Services.LoggerService;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

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

        [HttpGet]
        public async Task<String> Get( string login, string password ) {

            
            ClientData? clientData = await _context.ClientData.FirstOrDefaultAsync( user => user.Nickname.Equals(login) );
            if (clientData == null) {
                return JsonResponceFormat<String>.GetResponce(HttpStatusCode.BadRequest , "Login or password invalid", "");
            }

            String dk = _hashService.HexString(clientData.Salt + password);
            if( !dk.Equals(clientData.DerivedKey) )
            {
                return JsonResponceFormat<String>.GetResponce(HttpStatusCode.BadRequest, "Login or password invalid", "");
            }
            var clientModel = _mapper.Map<ClientModel>(clientData);


            
            
            return JsonResponceFormat<String>.GetResponce(HttpStatusCode.OK, "User information", JsonSerializer.Serialize(clientModel));
        }

        [HttpGet("recovery")]
        public async Task<String> GetRecovery(string login)
        {
            throw new Exception("Сервер відправки повідомлень ще не створений");
            return JsonResponceFormat<String>.GetResponce(HttpStatusCode.OK, "User information", JsonSerializer.Serialize(clientModel));
        }

        [HttpPost]
        public async Task<String> Post(ClientModel client)
        { 
            if (!ModelState.IsValid)
            {
                return JsonResponceFormat<ModelStateDictionary>.GetResponce(HttpStatusCode.BadRequest, "bad request", ModelState);
            }

            if(!client.isValid())
            {
                return JsonResponceFormat<String>.GetResponce(HttpStatusCode.BadRequest, "validate error", "");
            }

            var clientData = _mapper.Map<ClientData>(client);
            clientData.ClientID = Guid.NewGuid().ToString();
            clientData.Salt = _hashService.HexString(Guid.NewGuid().ToString());
            clientData.DerivedKey = _hashService.HexString(clientData.Salt + client.Password);

            // Додавання нового клієнта
            _context.ClientData.Add(clientData);

            await _context.SaveChangesAsync();

            return JsonResponceFormat<String>.GetResponce(HttpStatusCode.Created, "user created", clientData.ClientID);
        }

        [HttpDelete]
        public async Task<String> Delete(String login, String password)
        {
            ClientData? clientData = await _context.ClientData.FirstOrDefaultAsync(user => user.Nickname.Equals(login));
            if (clientData == null)
            {
                return JsonResponceFormat<String>.GetResponce(HttpStatusCode.BadRequest, "Login or password invalid", "");
            }
            String dk = _hashService.HexString(clientData.Salt + password);
            if (!dk.Equals(clientData.DerivedKey))
            {
                return JsonResponceFormat<String>.GetResponce(HttpStatusCode.BadRequest, "Login or password invalid", "");
            }

            if( clientData.ClientPasportData != null )
            {
                _context.ClientPasportData.Remove( clientData.ClientPasportData );
                clientData.PasportID = null;  
            }

            //Зделать неактивным все даные (недвижимость, компании)


            throw new Exception("Error");
        }
    }
}
