using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Client.API.Data.Entities
{
    [Table("client_data")]
    public class ClientData
    {
        [Key]
        [MaxLength(36)]
        [Column("id")]
        public string ClientID { get; set; } = default!;

        [MaxLength(36)]
        [Required]
        [Column("avatar")]
        public string Avatar { get; set; } = default!;

        [MaxLength(32)]
        [Required]
        [Column("real_name")]
        public string RealName { get; set; } = default!;

        [MaxLength(32)]
        [Column("nickname")]
        public string Nickname { get; set; } = default!;

        [MaxLength(32)]
        [Required]
        [Column("email")]
        public string Email { get; set; } = default!;

        [MaxLength(32)]
        [Required]
        [Column("phone")]
        public string Phone { get; set; } = default!;

        [Required]
        [Column("birthday")]
        public DateTime Birthday { get; set; }
        
        [Required]
        [Column("gender")]
        public bool Gender { get; set; }

        [MaxLength(32)]
        [Required]
        [Column("citizenship")]
        public string Citizenship { get; set; } = default!;

        [MaxLength(36)]
        [Required]
        [Column("salt")]
        public string Salt { get; set; } = default!;

        [MaxLength(36)]
        [Required]
        [Column("derived_key")]
        public string DerivedKey { get; set; } = default!;

        [MaxLength(32)]
        [Column("pasport_id")]
        public string? PasportID { get; set; } 
        
        [Required]
        [Column("is_verified")]
        public bool isVerified { get; set; } = false;
       
        [Required]
        [Column("balance")]
        public double Balance { get; set; } = 0.0;

        [ForeignKey("PasportID")]
        public ClientPasportData? ClientPasportData { get; set; }
    }
}
