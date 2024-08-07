
using System.Text.Json.Serialization;

namespace Token.API.Models
{

    public class LogModel
    {
        [JsonPropertyName("log_level")]
        public LogLevel Level { get; set; }
        [JsonPropertyName("message")]
        public string Message { get; set; }
        [JsonPropertyName("stacktrace")]
        public string? Stacktrace { get; set; }
    }

    public enum LogLevel
    {
        Trace, Debug, Information, Warning, Error, Critical
    }
}