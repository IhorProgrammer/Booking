using System.Buffers.Text;
using System.Security.Cryptography;
using System.Text;

namespace Token.API.Helpers
{
    public static class Sha256Helper
    {
        public static string ComputeHash(string codeVerifier)
        {
            using (var sha256 = SHA256.Create())
            {
                var codeVerifierBytes = Encoding.ASCII.GetBytes(codeVerifier);
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
