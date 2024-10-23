using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Residence.API.Data.DAO
{
    [Table("categories")]
    public class CategoriesDAO
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("categories_name")]
        public string CategoriesName { get; set; } = default!;

        [Required]
        [MaxLength(45)]
        [Column("image")]
        public string Image { get; set; } = default!;
    }
}
