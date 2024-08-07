
using System.Text.Json.Serialization;

namespace ProjectLibrary.Models
{

    public class LogModel
    {
        [JsonPropertyName("log_level")]
        public LogLevel Level { get; set; } = default!;
        [JsonPropertyName("message")]
        public string Message { get; set; } = default!;
        [JsonPropertyName("stacktrace")]
        public string? Stacktrace { get; set; } = default!;
    }

    public enum LogLevel
    {
        Trace, Debug, Information, Warning, Error, Critical
    }
}