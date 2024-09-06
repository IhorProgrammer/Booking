using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BookingLibrary.Services.TokenService.Model
{
    public class EncryptionDecryptionModel<T>
    {
        [JsonPropertyName("data")]
        public T Data { get; set; } = default!;
        [JsonPropertyName("user_agent")]
        public string UserAgent { get; set; } = default!;
    }
}
