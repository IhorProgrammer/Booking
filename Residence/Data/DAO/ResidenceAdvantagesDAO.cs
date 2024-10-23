using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Residence.API.Data.DAO
{
    [Table("residence_advantages")]
    public class ResidenceAdvantagesDAO
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("id_residence")]
        public int ResidenceId { get; set; }

        [Required]
        [Column("id_advantages")]
        public int AdvantagesId { get; set; }

        public AdvantagesDAO? Advantages { get; set; }
        public ResidenceDAO? Residence { get; set; }
    }
}
