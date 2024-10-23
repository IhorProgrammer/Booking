using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Residence.API.Data.DAO
{
    [Table("apartment_summary")]
    public class ApartmentSummaryDAO
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("id_apartment")]
        public int ApartmentId { get; set; }

        [Required]
        [Column("id_summary")]
        public int SummaryId { get; set; }

        public SummaryDAO? Summary { get; set; }
    }
}
