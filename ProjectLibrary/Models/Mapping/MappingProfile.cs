using AutoMapper;
using ProjectLibrary.Data.Entities;
using ProjectLibrary.Models.DTO;

namespace ProjectLibrary.Models.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile() {
            //Client
            CreateMap<ClientData, ClientModel>().ReverseMap();
            //Token
            CreateMap<TokenData, TokenModel>().ReverseMap();
        }
    }
}
