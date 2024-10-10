using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BookingLibrary.Data.DAO
{
    [Table("user_info")]
    public class UserInfoDAO
    {
        [Key]
        [MaxLength(36)]
        [Column("id_user")]
        public string UserID { get; set; } = default!;

        [Required]
        [MaxLength(254)]
        [Column("email")]
        public string Email { get; set; } = default!;

        public ICollection<TokenInfoDAO> Tokens { get; set; } = new List<TokenInfoDAO>();
    }
}
