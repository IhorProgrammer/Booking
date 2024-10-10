using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BookingLibrary.Data.DAO
{
    [Table("token_info")]
    public class TokenInfoDAO
    {
        [Key]
        [MaxLength(36)]
        [Column("id_token")]
        public string TokenID { get; set; } = default!;

        [Required]
        [MaxLength(36)]
        [Column("id_user")]
        public string UserID { get; set; } = default!;


        public UserInfoDAO? UserInfoData { get; set; }
    }
}
