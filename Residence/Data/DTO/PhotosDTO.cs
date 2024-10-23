using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Residence.API.Data.DTO
{
    public class PhotosDTO
    {
        [JsonPropertyName("url")]
        public string Url { get; set; } = default!;

        [JsonPropertyName("number")]
        public int Num { get; set; }
    }
}
