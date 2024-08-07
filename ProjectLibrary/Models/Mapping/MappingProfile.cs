using AutoMapper;
using ProjectLibrary.Data.Entities;
using ProjectLibrary.Models.DTO;

namespace ProjectLibrary.Models.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile() {
            CreateMap<ClientData, ClientModel>();
            CreateMap<ClientModel, ClientData>();

        }
    }
}
