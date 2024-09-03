using BookingLibrary.Helpers.Hash.HashTypes;

namespace BookingLibrary.Helpers.Hash.HashTypes
{
    public class Md5Hash : IHash
    {
        public string HashString(string text)
        {
            using var hasher = System.Security.Cryptography.MD5.Create();
            byte[] bytes = hasher.ComputeHash(System.Text.Encoding.UTF8.GetBytes(text));
            return Convert.ToHexString(bytes);
        }
    }
}
