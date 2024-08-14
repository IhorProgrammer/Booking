using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using ProjectLibrary.Models.TokenOptionsModel;
using System.Text;

namespace Token.API.Models
{
    public class TokenOptionsModel : ITokenOptionsModel
    {
        public string Token { get; set; } = default!;
        public string SecretKey { get; set; } = default!;
        public string UserAgent { get; set; } = default!;
        public int SaltID { get; set; } = default!;
        public DateTime Created { get; set; }

        public SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            // Переконатися, що всі частини ключа мають належний формат
            var keyString = $"{UserAgent}{Token}{SaltID}{SecretKey}";

            // Використовувати UTF8 для кодування, щоб забезпечити коректне кодування
            return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
        }
    }
}
