using Client.API.Models;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Text.Json;

namespace Client.API.Services.LoggerService
{
    public class LoggerManager : ILoggerManager
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public LoggerManager(IConfiguration configuration)
        {
            _configuration = configuration;
            _httpClient = new HttpClient();

        }

        private void Log( Exception exception, Models.LogLevel logLevel )
        {
            var logModel = new LogModel
            {
                Level = logLevel,
                Message = exception.Message,
                Stacktrace = String.IsNullOrEmpty(exception.StackTrace) ? "" : exception.StackTrace
            };
            var logContent = JsonSerializer.Serialize(logModel);
            var connection = _configuration.GetConnectionString("LoggerServerConnection");
            
            if (connection != null)
            {
                _httpClient.PostAsJsonAsync(connection, logModel).Wait();
            }
        }

        public void LogDebug(Exception exception)
        {
            Log( exception, Models.LogLevel.Debug );
        }

        public void LogError(Exception exception)
        {
            Log(exception, Models.LogLevel.Error);

        }
        public void LogInformation(Exception exception)
        {
            Log(exception, Models.LogLevel.Information);

        }
        public void LogWarning(Exception exception)
        {
            Log(exception, Models.LogLevel.Warning);
        }
    }
}
