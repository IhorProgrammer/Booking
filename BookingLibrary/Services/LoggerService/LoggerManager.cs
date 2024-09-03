using Microsoft.Extensions.Logging;
using ProjectLibrary.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace BookingLibrary.Services.LoggerService
{
    public class LoggerManager : ILoggerManager
    {
        private readonly HttpClient _httpClient = new HttpClient();
        public string LoggerServerConnection { get; private set; }

        public LoggerManager( string connection )
        {
            LoggerServerConnection = connection;
        }

        private void Log( Exception exception, LogLevel logLevel )
        {
            var logModel = new LogModel
            {
                Level = logLevel,
                Message = exception.Message,
                Stacktrace = String.IsNullOrEmpty(exception.StackTrace) ? "" : exception.StackTrace
            };
            var logContent = JsonSerializer.Serialize(logModel);
            var connection = LoggerServerConnection;


            if (connection != null)
            {
                _httpClient.PostAsJsonAsync(connection, logModel);
            }
        }

        public void LogDebug(Exception exception)
        {
            Log( exception, LogLevel.Debug );
        }

        public void LogError(Exception exception)
        {
            Log(exception, LogLevel.Error);

        }
        public void LogInformation(Exception exception)
        {
            Log(exception, LogLevel.Information);

        }
        public void LogWarning(Exception exception)
        {
            Log(exception, LogLevel.Warning);
        }
    }
}
