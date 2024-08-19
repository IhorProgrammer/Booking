using System.Text.Json.Serialization;

namespace Client.API.Models
{
    public class AuthModel
    {
        [JsonPropertyName("login")]
        public string Login { get; set; } = default!;

        [JsonPropertyName("password")]
        public string Password { get; set; } = default!;
    }
}
