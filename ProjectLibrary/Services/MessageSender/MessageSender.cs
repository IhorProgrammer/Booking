using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using ProjectLibrary.Models;
using ProjectLibrary.Services.JsonResponce;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ProjectLibrary.Services.MessageSender
{
    public class MessageSender : IMessageSender
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public MessageSender(IConfiguration configuration)
        {
            _httpClient = new HttpClient();
            _configuration = configuration;
        }

        void IMessageSender.Send(string jwt, MessageSenderTypes messageSender)
        {
            string? connection = _configuration.GetConnectionString("EmailServerConnection");
            if (connection == null) throw new ArgumentNullException("EmailServerConnection null", nameof(connection));

            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwt);
                httpClient.GetAsync($"{connection}/{(int)messageSender}").Wait();
            }
                
        }
    }
}
