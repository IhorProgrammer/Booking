using System.Text.Json.Serialization;

namespace Token.API.Models
{
    public class EncryptionResponceModel
    {
        [JsonPropertyName("iv")]
        public string IV { get; set; } = default!;

        [JsonPropertyName("encrypted")]
        public string Encrypted { get; set; } = default!;

    }
}
