using System.Text.Json.Serialization;
using static Mysqlx.Notice.Warning.Types;

namespace Logger.API.Models
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
}