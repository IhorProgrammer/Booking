namespace Token.API.Models
{
    public class TokenGenerationResponse
    {
        public string token { get; set; } = default!;
        public string salt { get; set; } = default!;
    }
}
