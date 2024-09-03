namespace BookingLibrary.Services.MessageSender
{
    public interface IMessageSender
    {
        public abstract void Send( string jwt, MessageSenderTypes massageSender );
    }
    public enum MessageSenderTypes
    {
        Unauthorized,
        Hacker,
        LoginAgain,
        Authorization,
        Registration
    }
}
