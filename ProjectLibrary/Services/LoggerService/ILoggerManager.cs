
namespace ProjectLibrary.Services.LoggerService
{
    public interface ILoggerManager
    {
        void LogInformation(Exception exception);
        void LogWarning(Exception exception);
        void LogDebug(Exception exception);
        void LogError(Exception exception);
    }
}
