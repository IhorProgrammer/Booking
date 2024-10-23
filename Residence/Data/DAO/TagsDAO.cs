using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Residence.API.Data.DAO
{
    [Table("tag")]
    public class TagsDAO
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("name")]
        public string Name { get; set; } = default!;

        public ICollection<TagApartmentDAO>? TagApartments { get; set; }
    }
}
