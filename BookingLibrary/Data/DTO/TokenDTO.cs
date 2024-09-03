
using System.Text.Json.Serialization;

namespace BookingLibrary.Data.DTO
{
    public class TokenDTO
    {
        [JsonPropertyName("id")]
        public String ID { get; set; } = default!;
        [JsonPropertyName("user_id")]
        public String UserID { get; set; } = default!;
        [JsonPropertyName("token_used")]
        public DateTime TokenUsed { get; set; }
    }

}
