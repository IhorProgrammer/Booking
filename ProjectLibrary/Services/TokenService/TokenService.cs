using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using ProjectLibrary.Models.EncryptionDecryptionModel;
using ProjectLibrary.Services.JsonResponce;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json;

namespace ProjectLibrary.Services.TokenService
{
    public class TokenService
    {

        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
            _httpClient = new HttpClient();
        }

        public string? GetJWTTokenByRequest(HttpRequest httpRequest)
        {
            var authHeader = httpRequest.Headers["Authorization"].ToString();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return null;
            }
            string? token = authHeader.Substring("Bearer ".Length).Trim();
            return token;
        }

        public bool TokenSubscription(HttpRequest httpRequest, string userAgent, string userId)
        {
            string? jwt = GetJWTTokenByRequest(httpRequest);

            string? connection = _configuration.GetConnectionString("TokenServerConnection");
            if (connection == null) throw new ArgumentNullException(nameof(connection));

            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwt);

                var jsonData = new
                {
                    agent = userAgent,
                    id = userId
                };

                StringContent jsonContent = new StringContent(JsonSerializer.Serialize(jsonData), Encoding.UTF8, "application/json");

                HttpResponseMessage response = httpClient.PutAsync(connection, jsonContent).Result;
                string? res = response.Content.ReadAsStringAsync().Result;
                if (res == null) throw new ArgumentNullException("Response tokenService == null. JsonData:" + jsonData);
                JsonResponceFormat<string>? jsonResponce = JsonSerializer.Deserialize<JsonResponceFormat<string>>(res);
                if (jsonResponce == null || (jsonResponce != null && jsonResponce.Meta.Code != 200)) return false;
                return true;
            }
        }

        public string? GetDecryptionJson(string jwt, string encrText, string iv)
        {
            string? connection = _configuration.GetConnectionString("TokenServerConnection");
            if (connection == null) throw new ArgumentNullException(nameof(connection));

            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwt);
                
                var str = $"{connection}/decryption/{Uri.EscapeDataString(encrText)}/{Uri.EscapeDataString(iv)}";
                HttpResponseMessage response = httpClient.GetAsync(str).Result;
                return response.Content.ReadAsStringAsync().Result;
            }
        }
    
        public EncryptionDecryptionModel<T>? DecryptionData<T>(HttpRequest httpRequest, string encryptionText, string iv)
        {
            try
            {
                string? jwt = GetJWTTokenByRequest(httpRequest);
                if (jwt == null) throw new Exception("Token invalid");

                string? decrRes = GetDecryptionJson(jwt, encryptionText, iv);
                if (decrRes == null) throw new ArgumentException("Token invalid");
                JsonResponceFormat<string>? jsonResponceFormat = JsonSerializer.Deserialize<JsonResponceFormat<string>>(decrRes);
                if (jsonResponceFormat == null) throw new ArgumentException("JsonResponceFormat null");
                if (jsonResponceFormat.Meta.Code != 200) throw new ArgumentException("Token invalid");
                EncryptionDecryptionModel<T>? deserialize = JsonSerializer.Deserialize<EncryptionDecryptionModel<T>>(jsonResponceFormat.Data);
                if (deserialize == null) throw new ArgumentException("Token invalid");
                return deserialize;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public string? GetTokenId(string jwt)
        {
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            JwtSecurityToken jwtToken = handler.ReadJwtToken(jwt);
            string? salt = jwtToken.Claims.FirstOrDefault(c => c.Type == "token_id")?.Value;
            return salt;
        }
    }
}
