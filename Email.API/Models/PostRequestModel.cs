using System.Text.Json.Serialization;

namespace Email.API.Models
{
    public class PostRequestModel
    {
        [JsonPropertyName("email")]
        public string Email { get; set; } = default!;
        [JsonPropertyName("id_user")]
        public string UserID { get; set; } = default!;
        [JsonPropertyName("id_token")]
        public string? TokenID { get; set; }

    }
}
