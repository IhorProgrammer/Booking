using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Residence.API.Data.DAO
{
    [Table("photos")]
    public class PhotosDAO
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("id_residence")]
        public int ResidenceId { get; set; }

        [Required]
        [MaxLength(254)]
        [Column("url")]
        public string Url { get; set; } = default!;

        [Required]
        [Column("num")]
        public int Num { get; set; }
    }
}
