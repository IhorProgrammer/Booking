using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Residence.API.Data.DAO
{
    [Table("advantages")]
    public class AdvantagesDAO
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("name")]
        public string Name { get; set; } = default!;

        public ICollection<ResidenceAdvantagesDAO>? ResidenceAdvantages { get; set; }
    }
}
