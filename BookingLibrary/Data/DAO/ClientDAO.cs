using BookingLibrary.Data.DTO;
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
        public string? GmailID { get; set; } = default!;
        
        [MaxLength(124)]
        [Column("avatar")]
        public string? Avatar { get; set; }

        [MaxLength(32)]
        [Column("given_name")]
        public string? GivenName { get; set; }

        [MaxLength(32)]
        [Column("family_name")]
        public string? FamilyName { get; set; }

        [MaxLength(16)]
        [Required]
        [Column("nickname")]
        public string Nickname { get; set; } = default!;

        [MaxLength(254)]
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

        [MaxLength(50)]
        [Column("citizenship")]
        public string? Citizenship { get; set; }

        [MaxLength(32)]
        [Required]
        [Column("salt")]
        public string Salt { get; set; } = default!;

        [MaxLength(32)]
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



        public ClientDAO ChangeData(ClientDTO client)
        {
            if( this.Avatar != client.Avatar && client.Avatar != null ) this.Avatar = client.Avatar;
            if( this.GivenName != client.GivenName && client.GivenName != null ) this.GivenName = client.GivenName;
            if( this.FamilyName != client.FamilyName && client.FamilyName != null ) this.FamilyName = client.FamilyName;
            if( this.Nickname != client.NewNickname && client.NewNickname != null ) this.Nickname = client.NewNickname; // Видати помилку якщо nickname є
            if( this.Phone != client.Phone && client.Phone != null ) this.Phone = client.Phone;
            // Birthday і Gender не повина змінюватись

            return this;
        }


        
    }
}
