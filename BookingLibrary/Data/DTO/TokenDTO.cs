
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BookingLibrary.Data.DTO
{
    public class TokenDTO
    {
        [JsonPropertyName("id")]
        public String TokenID { get; set; } = default!;
        [JsonPropertyName("user_id")]
        public String UserID { get; set; } = default!;
        [JsonPropertyName("token_used")]
        public DateTime TokenUsed { get; set; }
        [JsonPropertyName("token_created")]
        public DateTime TokenCreated { get; set; }
    }

}
