using Residence.API.Data.DAO;
using Residence.API.Data.DTO;
using System.Text.Json.Serialization;

namespace Residence.API.Models
{
    public class ResidenceInfo
    {
        [JsonPropertyName("residence")]
        public ResidenceDTO? Residence { get; set; } = default!;
        [JsonPropertyName("advantages")]
        public List<AdvantagesDTO?>? Advantages { get; set; } = default!;
        [JsonPropertyName("photos")]
        public List<PhotosDTO?>? Photos { get; set; } = default!;
        [JsonPropertyName("apartments")]
        public List<ApartmentInfo> Apartments { get; set; } = new List<ApartmentInfo>();
    }

    public class ApartmentInfo
    {
        [JsonPropertyName("info")]
        public ApartmentDTO Apartment { get; set; } = default!;
        [JsonPropertyName("summary")]
        public List<SummaryDTO?>? Summary { get; set; } = default!;
        [JsonPropertyName("tags")]
        public List<TagsDTO?>? Tags { get; set; } = default!;

    }
}
