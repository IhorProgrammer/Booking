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
        public String ID {  get; set; } = default!;
        [Required]
        [MaxLength(36)]
        [Column("id_user")]
        public String UserID { get; set; } = default!;
        [Required]
        [Column("token_created")]
        public DateTime TokenCreated {  get; set; }
        [Required]
        [Column("token_expires")]
        public DateTime TokenExpires { get; set; }
        [Required]
        [Column("token_type")]
        public int TokenType { get; set; }
    }
}
