using AutoMapper;
using BookingLibrary.Data.DAO;
using BookingLibrary.Data.DTO;


namespace BookingLibrary.Data.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile() {
            //Client
            CreateMap<ClientDAO, ClientDTO>().ReverseMap();
            //Token
            CreateMap<TokenDAO, TokenDTO>().ReverseMap();
        }
    }
}
