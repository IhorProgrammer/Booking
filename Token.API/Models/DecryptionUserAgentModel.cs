using System.Text.Json.Serialization;

namespace Token.API.Models
{
    public class DecryptionUserAgentModel
    {
        [JsonPropertyName("user_agent")]
        public string UserAgent { get; set; } = default!;
    }
}
