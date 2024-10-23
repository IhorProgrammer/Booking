using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Residence.API.Data.DAO
{
    [Table("residence")]
    public class ResidenceDAO
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

        [ForeignKey("CategoryId")]
        public CategoriesDAO? Category { get; set; }
        
        public ICollection<ResidenceAdvantagesDAO>? ResidenceAdvantages { get; set; }
    }
}
