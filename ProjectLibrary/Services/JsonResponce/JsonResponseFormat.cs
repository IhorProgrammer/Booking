using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ProjectLibrary.Services.JsonResponce
{

    public class JsonResponseFormat<T>
    {
        [JsonPropertyName("data")]
        public T Data { get; set; } = default!;
        [JsonPropertyName("meta")]
        public MetaData Meta { get; set; } = default!;


        //public static string GetResponce(HttpStatusCode code, string message, T data)
        //{
        //    var jf = new JsonResponceFormat<T>();
        //    jf.Data = data;
        //    jf.Meta = new MetaData((int)code, message);
        //    string jsonString = JsonSerializer.Serialize(jf);

        //    return jsonString;
        //}



        public class MetaData
        {
            [JsonPropertyName("created")]
            public DateTime Timestamp { get; set; }
            [JsonPropertyName("status")]
            public string Message { get; set; }
            [JsonPropertyName("code")]
            public int Code { get; set; }

            public MetaData(int code, string message)
            {
                Timestamp = DateTime.UtcNow;
                Code = code;
                Message = message;
            }
        }
    }
}
