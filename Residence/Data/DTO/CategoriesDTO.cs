using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Residence.API.Data.DTO
{
    public class CategoriesDTO
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        
        [JsonPropertyName("categories_name")]
        public string CategoriesName { get; set; } = default!;
        
        [JsonPropertyName("image")]
        public string Image { get; set; } = default!;
    }
}
