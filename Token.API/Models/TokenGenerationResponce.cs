namespace Token.API.Models
{
    public class TokenGenerationResponce
    {
        public string token { get; set; } = default!;
        public string salt { get; set; } = default!;
    }
}
