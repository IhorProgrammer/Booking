using System.Text.Json.Serialization;

namespace BookingLibrary.JsonResponce
{

    public class JsonResponseFormat<T>
    {
        [JsonPropertyName("data")]
        public T Data { get; set; } = default!;
        [JsonPropertyName("meta")]
        public MetaData Meta { get; set; } = default!;

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

        public object ToObject() => (object)this;
    }
}
