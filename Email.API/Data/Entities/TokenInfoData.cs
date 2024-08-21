using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using ProjectLibrary.Data.Entities;

namespace Email.API.Data.Entities
{
    [Table("token_info")]
    public class TokenInfoData
    {
        [Key]
        [MaxLength(36)]
        [Column("id_token")]
        public string TokenID { get; set; } = default!;

        [Required]
        [MaxLength(36)]
        [Column("id_user")]
        public string UserID { get; set; } = default!;


        public UserInfoData? UserInfoData { get; set; }
    }
}
