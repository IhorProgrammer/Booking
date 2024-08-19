using System.Text.Json.Serialization;

namespace Token.API.Models
{
    public class PutRequestModel
    {
        [JsonPropertyName("agent")]
        public string UserAgent { get; set; } = default!;
        [JsonPropertyName("id")]
        public string UserId { get; set; } = default!;
    }
}
