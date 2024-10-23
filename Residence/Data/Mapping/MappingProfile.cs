using AutoMapper;
using Residence.API.Data.DAO;
using Residence.API.Data.DTO;

namespace Residence.API.Data.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile() {
            CreateMap<CategoriesDAO, CategoriesDTO>().ReverseMap();
            CreateMap<AdvantagesDAO, AdvantagesDTO>().ReverseMap();
            CreateMap<ResidenceDAO, ResidenceDTO>().ReverseMap();
            CreateMap<PhotosDAO, PhotosDTO>().ReverseMap();
            CreateMap<ApartmentDAO, ApartmentDTO>().ReverseMap();
            CreateMap<TagsDAO, TagsDTO>().ReverseMap();
            CreateMap<SummaryDAO, SummaryDTO>().ReverseMap();


        }
    }
}
