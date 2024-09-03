using BookingLibrary.Helpers.Hash.HashTypes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookingLibrary.Data.DAO
{
    [Table("client_data")]
    public class ClientDAO
    {
        [Key]
        [MaxLength(36)]
        [Column("id")]
        public string ClientID { get; set; } = default!;
        
        [Column("gmail_id")]
        [MaxLength(24)]
        public string GmailID { get; set; } = default!;
        
        [MaxLength(124)]
        [Column("avatar")]
        public string? Avatar { get; set; }

        [MaxLength(32)]
        [Column("given_name")]
        public string? GivenName { get; set; }

        [MaxLength(32)]
        [Column("family_name")]
        public string? FamilyName { get; set; }

        [MaxLength(32)]
        [Column("nickname")]
        public string? Nickname { get; set; }

        [MaxLength(32)]
        [Required]
        [Column("email")]
        public string Email { get; set; } = default!;

        [MaxLength(32)]
        [Column("phone")]
        public string? Phone { get; set; }

        [Column("birthday")]
        [DataType(DataType.Date)]
        public DateTime? Birthday { get; set; }
        
        [Column("gender")]
        public bool? Gender { get; set; }

        [MaxLength(32)]
        [Column("citizenship")]
        public string? Citizenship { get; set; }

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
        public ClientPasportDAO? ClientPasportData { get; set; }


        public bool ValidatePassword(string enteredPassword, IHash hashService)
        {
            string enteredDerivedKey = hashService.HashString(Salt + enteredPassword);
            return enteredDerivedKey.Equals(DerivedKey);
        }

    }
}
