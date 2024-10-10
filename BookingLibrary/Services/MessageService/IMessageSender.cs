using Microsoft.Extensions.Logging;

namespace BookingLibrary.Services.MessageSender
{
    public interface IMessageSender
    {
        public abstract void Send( string jwt, MessageSenderTypes massageSender );
    }
    public class MessageSenderTypes
    {
        public static readonly MessageSenderTypes Hacker = new() { LogLevel = LogLevel.Critical, Name = nameof(Hacker) };
        public static readonly MessageSenderTypes LoginAgain = new() { LogLevel = LogLevel.Warning, Name = nameof(LoginAgain) };
        public static readonly MessageSenderTypes Authorization = new() { LogLevel = LogLevel.Information, Name = nameof(Authorization) };
        public static readonly MessageSenderTypes Registration = new() { LogLevel = LogLevel.Information, Name = nameof(Registration) };
        public static readonly MessageSenderTypes Unauthorized = new() { LogLevel = LogLevel.Information, Name = nameof(Unauthorized) };

        public LogLevel LogLevel;
        public string Name;

    }


}
