using System.Text.Json.Serialization;

namespace Residence.API.Data.DTO
{
    public class ResidenceDTO
    {
        [JsonPropertyName("id_residence")]
        public int Id { get; set; }
        [JsonPropertyName("id_category")]
        public int CategoryId { get; set; }
        [JsonPropertyName("residence_name")]
        public string ResidenceName { get; set; } = default!;
        [JsonPropertyName("rate")]
        public int Rate { get; set; }
        [JsonPropertyName("popularity")]
        public int Popularity { get; set; }
        [JsonPropertyName("address")]
        public string Address { get; set; } = default!;
        [JsonPropertyName("price")]
        public double Price { get; set; }
        [JsonPropertyName("promotional_price")]
        public double PromotionalPrice { get; set; }
    }
}