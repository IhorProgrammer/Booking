using AutoMapper;
using BookingLibrary.Data.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using Residence.API.Data.DAO;
using Residence.API.Data.DTO;
using Residence.API.Models;
using Residence.API.ResponseFormatExtended;
using Residence.Data;

namespace Residence.Controllers
{
    [ApiController]
    [Route("")]
    public class ResidenceController : Controller
    {
        private ResidenceDBContext dbContext;
        private readonly IMapper _mapper;

        public ResidenceController(ResidenceDBContext residenceDBContext, IMapper mapper)
        {
            dbContext = residenceDBContext;
            _mapper = mapper;
        }


        [HttpGet]
        public object SearchResidence( string? search, int? category, int? people, int? price_order, bool? popularity_order, int? count, int? offset )
        {
            var searchParam = new MySqlParameter("@search", search ?? "");
            var idCategoryParam = new MySqlParameter("@id_category", category ?? (object)DBNull.Value);
            var maxPeopleParam = new MySqlParameter("@max_people", people ?? (object)DBNull.Value);
            var priceOrderParam = new MySqlParameter("@price_order", price_order ?? (object)DBNull.Value);
            var popularityOrderParam = new MySqlParameter("@popularity_order", popularity_order ?? false);
            var resultCountParam = new MySqlParameter("@result_count", count ?? 3);
            var offsetValueParam = new MySqlParameter("@offset_value", offset ?? 0);

            var residences = dbContext.ResidencesSearch
                .FromSqlRaw("CALL GetResidences(@search, @id_category, @max_people, @price_order, @popularity_order, @result_count, @offset_value)",
                    searchParam, idCategoryParam, maxPeopleParam, priceOrderParam, popularityOrderParam, resultCountParam, offsetValueParam)
                .ToList();
            return ResponseFormat.GetResponceJson(ResponseFormat.RESIDENCE_SEARCH_GET, residences);
        }

        [HttpGet("residence/{url}")]
        public async Task<object> GetImageResidence(string url)
        {
            string filePath = Path.Combine("images/residence", url);

            if (System.IO.File.Exists(filePath))
            {
                var imageBytes = await System.IO.File.ReadAllBytesAsync(filePath);
                return File(imageBytes, "image/jpeg");
            }
            else
            {
                return NotFound("Зображення не знайдено.");
            }
        }

        [HttpGet("apartment/{url}")]
        public async Task<object> GetImageApartment(string url)
        {
            string filePath = Path.Combine("images/residence/apartment", url);

            if (System.IO.File.Exists(filePath))
            {
                var imageBytes = await System.IO.File.ReadAllBytesAsync(filePath);
                return File(imageBytes, "image/jpeg");
            }
            else
            {
                return NotFound("Зображення не знайдено.");
            }
        }


        [HttpGet("residence-info/{id}")]
        public object GetInfoResidence(int id) 
        {
            
            ResidenceInfo residenceInfo = new ResidenceInfo();
            var residence = dbContext.Residences.Find(id);
            if (residence == null) return null; 

            residenceInfo.Residence = _mapper.Map<ResidenceDTO>(residence);

            var advantages = dbContext.ResidenceAdvantages
                .Where(a => a.ResidenceId == id)
                .Include(a => a.Advantages)
                .Select(res => res.Advantages)
                .ToList();

            residenceInfo.Advantages = _mapper.Map<List<AdvantagesDTO?>?>(advantages);

            var photos = dbContext.Photos.Where(p => p.ResidenceId == id).ToList();
            residenceInfo.Photos = _mapper.Map<List<PhotosDTO?>?>(photos);


            var apartments = dbContext.Apartments.Where(a => a.ResidenceId == id).ToList();
            foreach (var apartment in apartments)
            {
                var summary = dbContext.ApartmentSummaries.Where(aps => aps.ApartmentId == apartment.Id).Include(s => s.Summary).Select(s => s.Summary).ToList();
                var tags = dbContext.TagApartments.Where(taa => taa.ApartmentId == apartment.Id).Include(t => t.Tags).Select(t => t.Tags).ToList(); 

                residenceInfo.Apartments.Add
                    (
                        new ApartmentInfo() { 
                            Apartment = _mapper.Map<ApartmentDTO>(apartment), 
                            Summary = _mapper.Map<List<SummaryDTO?>>(summary), 
                            Tags = _mapper.Map<List<TagsDTO?>?>(tags) 
                        }
                    );
            }

            return residenceInfo;
        }
    }
}
