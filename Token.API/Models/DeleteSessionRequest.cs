using System.Text.Json.Serialization;

namespace Token.API.Models
{
    public class DeleteSessionRequest
    {
        [JsonPropertyName("token_id")]
        public string TokenID { get; set; } = default!;
    }
}
