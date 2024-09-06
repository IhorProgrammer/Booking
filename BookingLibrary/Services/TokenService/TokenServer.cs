using BookingLibrary.JsonResponce;
using BookingLibrary.Services.TokenService.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BookingLibrary.Services.TokenService
{
    public class TokenServer 
    {
        private readonly IConfiguration configuration;


        public TokenServer(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        //public string GetTokenID(string userAgent, string jwt)
        //{
        //    throw new NotImplementedException();
        //}
        public EncryptionDecryptionModel<T> Decryption<T>(string data, string iv, string jwt)
        {
            string? connection = configuration.GetConnectionString("TokenServerConnection");
            if (connection == null) throw ResponseFormat.TOKEN_SERVER_CONNECTION_NULL.Exception;

            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwt);

                var str = $"{connection}/decryption/{Uri.EscapeDataString(data)}/{Uri.EscapeDataString(iv)}";
                HttpResponseMessage response = httpClient.GetAsync(str).Result;
                string res = response.Content.ReadAsStringAsync().Result;

                JsonResponseFormat<string>? jsonResponce = JsonSerializer.Deserialize<JsonResponseFormat<string>>(res);
                if( jsonResponce != null && jsonResponce.Meta.Code == 200)
                {
                    return JsonSerializer.Deserialize<EncryptionDecryptionModel<T>>(jsonResponce.Data) ?? throw ResponseFormat.DECRYPTION_SERVER_EMPTY_STRING.Exception;
                }
                if( jsonResponce != null && jsonResponce.Meta != null ) throw ResponseFormat.DECRYPTION_SERVER_EMPTY_STRING.Exception;
                throw ResponseFormat.TOKEN_SERVER_CONNECTION_NULL.Exception;

            }
        } 

        public bool Subscription(string userAgent, string userId, string userEmail, string jwt)
        {
            string? connection = configuration.GetConnectionString("TokenServerConnection");
            if (connection == null) throw ResponseFormat.TOKEN_SERVER_CONNECTION_NULL.Exception;

            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwt);

                var jsonData = new
                {
                    agent = userAgent,
                    id = userId,
                    email = userEmail,
                };

                StringContent jsonContent = new StringContent(JsonSerializer.Serialize(jsonData), Encoding.UTF8, "application/json");

                HttpResponseMessage response = httpClient.PutAsync(connection, jsonContent).Result;
                string? res = response.Content.ReadAsStringAsync().Result;
                if (res == null) throw new ArgumentNullException("Response tokenService == null. JsonData:" + jsonData);
                JsonResponseFormat<string>? jsonResponce = JsonSerializer.Deserialize<JsonResponseFormat<string>>(res);
                if (jsonResponce == null || (jsonResponce != null && jsonResponce.Meta.Code != 200)) return false;
                return true;
            }
        }
    }
}
