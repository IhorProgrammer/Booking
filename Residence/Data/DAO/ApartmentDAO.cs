using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Residence.API.Data.DAO
{

    [Table("apartment")]
    public class ApartmentDAO
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("id_residence")]
        public int ResidenceId { get; set; }

        [Required]
        [Column("apartment_name")]
        public string ApartmentName { get; set; } = default!;

        [Required]
        [MaxLength(254)]
        [Column("url_image")]
        public string Url { get; set; } = default!;

        [Required]
        [Column("max_people")]
        public int MaxPeople { get; set; }

        [Required]
        [Column("price")]
        public double Price { get; set; }

        [Required]
        [Column("promotional_price")]
        public double PromotionalPrice { get; set; }

    }
}
