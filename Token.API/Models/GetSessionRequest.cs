using System.Text.Json.Serialization;

namespace Token.API.Models
{
    public class GetSessionRequest
    {
        [JsonPropertyName("client_id")]
        public string ClientID { get; set; } = default!;
    }
}
