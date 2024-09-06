using BookingLibrary.Data;
using BookingLibrary.Data.DAO;
using BookingLibrary.Helpers.AES;
using BookingLibrary.JsonResponce;
using System.Text.Json;
using Token.API.Contracts;
using Token.API.Models;

namespace Token.API.Services.Tokens
{
    public class TokenServiceED : TokenService
    {
        public TokenDAO TokenData { get; private set; } = default!;

        private TokenServiceED(TokenDBContext context, HttpRequest request) : base(context, request){}

        public static TokenServiceED GetTokenServiceED(TokenDBContext context, HttpRequest request)
        {
            TokenServiceED tokenServiceED = new TokenServiceED(context, request);
            tokenServiceED.setData();
            return tokenServiceED;
        }

        private void setData()
        {
            TokenCheckRequest tokenCheckRequest = new TokenCheckRequest(Context);
            tokenCheckRequest.Token = Token;
            string tokenID = tokenCheckRequest.TokenId;
            TokenDAO? tokenData = tokenCheckRequest.TokenData.Clone();
            if (tokenData == null) throw ResponseFormat.TOKEN_DATA_NULL.Exception;
            TokenData = tokenData;
        }

        public string JsonDecrypt(string data, string iv)
        {
            string textJson = AES.Decrypt(Convert.FromBase64String(Uri.UnescapeDataString(data)), Convert.FromBase64String(Uri.UnescapeDataString(iv)), TokenData.Salt);
            return textJson;
        }

        public string UserAgentByJsonDecrypt( string textJson )
        {
            DecryptionUserAgentModel? decryptionModel = JsonSerializer.Deserialize<DecryptionUserAgentModel>(textJson);
            if (decryptionModel == null) throw ResponseFormat.DECRYPTION_NULL.Exception;
            return decryptionModel.UserAgent;
        }

    }
}
