using System.Text.Json.Serialization;

namespace Residence.API.Data.DTO
{
    public class SummaryDTO
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("summary_info_name")]
        public string Name { get; set; } = default!;

    }
}
