using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace ProjectLibrary.Models.DTO
{
    public class TokenModel
    {
        [JsonPropertyName("id")]
        public String ID { get; set; } = default!;
        [JsonPropertyName("user_id")]
        public String UserID { get; set; } = default!;

    }

}
