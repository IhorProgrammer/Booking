using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Residence.API.Data.DAO
{
    public class ResidenceSearchDAO
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("id_category")]
        public int CategoryId { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("residence_name")]
        public string ResidenceName { get; set; } = default!;
        
        [Required]
        [Column("rate")]
        public int Rate { get; set; }
        
        [Required]
        [Column("popularity")]
        public int Popularity { get; set; }

        [Required]
        [MaxLength(254)]
        [Column("address")]
        public string Address { get; set; } = default!;
        
        [Required]
        [Column("price")]
        public double Price { get; set; }
        
        [Required]
        [Column("promotional_price")]
        public double PromotionalPrice { get; set; }

        [Column("url")] 
        public string? ImageUrl { get; set; } 
    }
}
