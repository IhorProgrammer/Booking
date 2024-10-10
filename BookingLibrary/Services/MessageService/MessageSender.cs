
namespace BookingLibrary.Services.MessageSender
{
    public class MessageSender : IMessageSender
    {
        private readonly string Connection;
        private readonly HttpClient _httpClient = new HttpClient();

        public MessageSender(string connection)
        {
            Connection = connection;
        }

        void IMessageSender.Send(string jwt, MessageSenderTypes messageSender)
        {
            

            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwt);
                httpClient.GetAsync($"{Connection}/{messageSender.Name}").Wait();
            }
        }
    }
}
