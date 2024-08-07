using AutoMapper;
using Client.API.Data;
using Client.API.Data.Entities;
using Client.API.Models.DTO;
using Client.API.Services;
using Client.API.Services.Hash;
using Client.API.Services.LoggerService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Client.API.Controllers
{
    [Route("api/client")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly ClientContext _context;
        private readonly IHashService _hashService;
        private readonly ILoggerManager _loggerManager;
        private readonly IMapper _mapper;

        public ClientController(ClientContext context, IHashService hashService, ILoggerManager loggerManager, IMapper mapper)
        {
            _context = context;
            _hashService = hashService;
            _loggerManager = loggerManager;
            _mapper = mapper;
        }

        [HttpGet("{id}")]
        public async Task<String> Get( string id ) {
            ClientData? clientData = await _context.ClientData.FindAsync( id );
            if (clientData == null) {
                return JsonResponceFormat<String>.GetResponce("failed", 200, "user not found", "");
            }
            var clientModel = _mapper.Map<ClientModel>(clientData);
            return JsonResponceFormat<String>.GetResponce("success", 200, "User information", JsonSerializer.Serialize(clientModel));
        }

        [HttpPost]
        public async Task<String> Post(ClientModel client)
        { 
            if (!ModelState.IsValid)
            {
                return JsonResponceFormat<ModelStateDictionary>.GetResponce("failed", 401, "bad request", ModelState);
            }

            if(!client.isValid())
            {
                return JsonResponceFormat<String>.GetResponce("failed", 401, "validate error", "");
            }

            var clientData = _mapper.Map<ClientData>(client);
            clientData.ClientID = Guid.NewGuid().ToString();
            clientData.Salt = _hashService.HexString(Guid.NewGuid().ToString());
            clientData.DerivedKey = _hashService.HexString(clientData.Salt + client.Password);

            // Додавання нового клієнта
            _context.ClientData.Add(clientData);

            await _context.SaveChangesAsync();
            return JsonResponceFormat<String>.GetResponce("success", 200, "user created", clientData.ClientID);
        }
        [HttpDelete]
        public async Task<String> Delete(String token, String password)
        {
            // удаление даных паспорта
            throw new Exception("Error");
        }


    }
}
