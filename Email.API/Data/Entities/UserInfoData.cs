using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Email.API.Data.Entities
{
    [Table("user_info")]
    public class UserInfoData
    {
        [Key]
        [MaxLength(36)]
        [Column("id_user")]
        public string UserID { get; set; } = default!;

        [Required]
        [MaxLength(254)]
        [Column("email")]
        public string Email { get; set; } = default!;

        public ICollection<TokenInfoData> Tokens { get; set; } = new List<TokenInfoData>();
    }
}
