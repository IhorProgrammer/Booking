using Email.API.Models;
using Email.API.Services.Email;

namespace Email.API.MessageSender
{
    public class MessageSender
    {
        private readonly IEmail _email;

        public MessageSender(IEmail email)
        {
            _email = email;
        }

        public bool Unauthorized(EmailModel model)
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            string path = Path.Combine(currentDirectory, "MessageSender\\Answers\\Unauthorized.html");
            string Body = File.ReadAllText(Path.Combine(currentDirectory, "MessageSender\\Answers\\Unauthorized.html"));

            model.Body = Body;
            model.Subject = "They are trying to hack your account";

            _email.Send(model);
            return true;
        }


    }
}
