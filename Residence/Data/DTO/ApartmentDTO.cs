using System.Text.Json.Serialization;

namespace Residence.API.Data.DTO
{
    public class ApartmentDTO
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("apartment_name")]
        public string ApartmentName { get; set; } = default!;

        [JsonPropertyName("image")]
        public string Url { get; set; } = default!;

        [JsonPropertyName("max_people")]
        public int MaxPeople { get; set; }

        [JsonPropertyName("price")]
        public double Price { get; set; }

        [JsonPropertyName("promotional_price")]
        public double PromotionalPrice { get; set; }

    }
}
