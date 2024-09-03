using System.Buffers.Text;
using System.Security.Cryptography;
using System.Text;

namespace BookingLibrary.Helpers.Hash.HashTypes
{
    public class Sha256Hash : IHash
    {
        public string HashString(string text)
        {
            using (var sha256 = SHA256.Create())
            {
                var codeVerifierBytes = Encoding.ASCII.GetBytes(text);
                var hashBytes = sha256.ComputeHash(codeVerifierBytes);

                // Кодування хешу в Base64Url
                var codeChallenge = Base64UrlEncode(hashBytes);
                return codeChallenge;
            }
        }

        private static string Base64UrlEncode(byte[] input)
        {
            var base64 = Convert.ToBase64String(input);
            var base64Url = base64
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');
            return base64Url;
        }
    }
}
