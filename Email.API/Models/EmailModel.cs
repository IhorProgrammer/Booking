using System.Text.Json.Serialization;

namespace Email.API.Models
{
    public class EmailModel
    {
        [JsonPropertyName("email_from")]
        public string From { get; set; } = default!;
        [JsonPropertyName("email_to")]
        public string To { get; set; } = default!;
        [JsonPropertyName("subject")]
        public string Subject { get; set; } = default!;
        [JsonPropertyName("body")]
        public string Body { get; set; } = default!;
        [JsonPropertyName("from_name")]
        public string FromName { get; set; } = default!;
        [JsonPropertyName("to_name")]
        public string ToName { get; set; } = default!;
        [JsonPropertyName("from_password")]
        public string FromPassword { get; set; } = default!;
    }
}
