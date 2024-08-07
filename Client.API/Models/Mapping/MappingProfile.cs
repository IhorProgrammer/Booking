using AutoMapper;
using Client.API.Data.Entities;
using Client.API.Models.DTO;

namespace Client.API.Models.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile() {
            CreateMap<ClientData, ClientModel>();
            CreateMap<ClientModel, ClientData>();

        }
    }
}
