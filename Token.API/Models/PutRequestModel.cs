using System.Text.Json.Serialization;

namespace Token.API.Models
{
    public class PutRequestModel
    {
        [JsonPropertyName("agent")]
        public string? UserAgent { get; set; }
        [JsonPropertyName("id")]
        public string UserId { get; set; } = default!;
        [JsonPropertyName("token_id")]
        public string? TokenId { get; set; }
        [JsonPropertyName("email")]
        public string? Email { get; set; }
    }
}
