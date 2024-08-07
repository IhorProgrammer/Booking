using System.Text.Json;
using System.Text.Json.Serialization;

namespace Client.API.Services
{

    public class JsonResponceFormat<T>
    {
        [JsonPropertyName("data")]
        public T Data { get; set; }
        [JsonPropertyName("meta")]
        public MetaData Meta { get; set; }


        public static string GetResponce(string status, int code, string message, T data)
        {
            var jf = new JsonResponceFormat<T>();
            jf.Data = data;
            jf.Meta = new MetaData( status, code, message );
            string jsonString = JsonSerializer.Serialize<JsonResponceFormat<T>>(jf);

            return jsonString;
        }

        public class MetaData
        {
            [JsonPropertyName("created")]
            public DateTime Timestamp { get; set; }
            [JsonPropertyName("status")]
            public string Status { get; set; }
            [JsonPropertyName("message")]
            public string Message { get; set; }
            [JsonPropertyName("code")]
            public int Code { get; set; }

            public MetaData(string status, int code, string message)
            {
                Timestamp = DateTime.UtcNow;
                Status = status;
                Code = code;
                Message = message;
            }
        }
    }
}
