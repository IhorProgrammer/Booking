using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Residence.API.Data.DAO
{
    [Table("tag_apartment")]
    public class TagApartmentDAO
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("id_apartment")]
        public int ApartmentId { get; set; }

        [Required]
        [Column("id_tag")]
        public int TagId { get; set; }

        public TagsDAO? Tags { get; set; }
    }

}
