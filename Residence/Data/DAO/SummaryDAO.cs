using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Residence.API.Data.DAO
{
    [Table("summary_info")]
    public class SummaryDAO
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("summary_info_name")]
        public string Name { get; set; } = default!;

        public ICollection<ApartmentSummaryDAO>? ApartmentSummaries { get; set; }
    }
}
