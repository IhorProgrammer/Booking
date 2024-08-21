using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectLibrary.Data.Entities
{
    [Table("tokens")]
    public class TokenData
    {
        [Key]
        [MaxLength(36)]
        [Column("id")]
        public String TokenID { get; set; } = default!;
        [MaxLength(36)]
        [Column("id_user")]
        public String? UserID { get; set; }
        [Required]
        [Column("token_created")]
        public DateTime TokenCreated {  get; set; }
        [Required]
        [Column("token_used")]
        public DateTime TokenUsed { get; set; }
        [Required]
        [Column("salt")]
        [MaxLength(36)]
        public String Salt { get; set; } = default!;
    }
}
