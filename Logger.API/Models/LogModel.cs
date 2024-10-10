using System.Text.Json.Serialization;
using static Mysqlx.Notice.Warning.Types;

namespace Logger.API.Models
{

    public class LogModel
    {
        [JsonPropertyName("log_level")]
        public LogLevel Level { get; set; } = default!;
        [JsonPropertyName("log_logger")]
        public string LogLogger { get; set; } = default!;
        [JsonPropertyName("message")]
        public string Message { get; set; } = default!;
        [JsonPropertyName("stacktrace")]
        public string? Stacktrace { get; set; } = default!;
    }
}