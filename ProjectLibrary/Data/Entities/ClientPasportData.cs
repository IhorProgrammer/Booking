using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProjectLibrary.Data.Entities
{
    [Table("client_pasport_data")]
    public class ClientPasportData
    {
        [Key]
        [MaxLength(32)]
        [Column("id")]
        public string ClientPasportDataID { get; set; } = default!;


        [MaxLength(64)]
        [Required]
        [Column("first_name_hash")]
        public string FirstNameHash { get; set; } = default!;

        [MaxLength(64)]
        [Required]
        [Column("last_name_hash")]
        public string LastNameHash { get; set; } = default!;

        [MaxLength(64)]
        [Required]
        [Column("expiry_date_hash")]
        public string ExpiryDateHash { get; set; } = default!;

        [MaxLength(64)]
        [Required]
        [Column("country_of_issule_hash")]
        public string CountryOfIssuleHash { get; set; } = default!;
        
        [MaxLength(64)]
        [Required]
        [Column("pasport_number_hash")]
        public string PasportNumberHash { get; set; } = default!;

        [MaxLength(32)]
        [Required]
        [Column("salt")]
        public string Salt { get; set; } = default!;

        public ClientData? ClientData { get; set; }
    }
}
