using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Residence.API.Data.DAO;
using Residence.API.Data.DTO;
using Residence.API.ResponseFormatExtended;
using Residence.Data;

namespace Residence.API.Controllers
{
    [ApiController]
    [Route("categories")]
    public class ResidenceCategoriesController : Controller
    {

        private ResidenceDBContext dbContext;
        private readonly IMapper _mapper;

        public ResidenceCategoriesController(ResidenceDBContext residenceDBContext, IMapper mapper)
        {
            dbContext = residenceDBContext;
            _mapper = mapper;
        }

        [HttpGet]
        public object GetCategories()
        {
            List<CategoriesDAO> listCategoriesDAO = dbContext.Categories.ToList();
            return ResponseFormat.GetResponceJson(ResponseFormat.RESIDENCE_CATEGORIES_GET, _mapper.Map<List<CategoriesDTO>>(listCategoriesDAO));
        }

        [HttpGet("{url}")]
        public async Task<object> GetImage(string url)
        {
            string filePath = Path.Combine("images/categories", url);

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

    }
}
