using Email.API.Models;
using Email.API.Services.Email;
using ProjectLibrary.Services.MessageSender;

namespace Email.API.MessageSender
{
    public class MessageSender
    {
        private readonly IEmail _email;

        public MessageSender(IEmail email)
        {
            _email = email;
        }


        public bool SendByType(EmailModel model, MessageSenderTypes types ) 
        {
            switch (types) 
            { 
                case MessageSenderTypes.Unauthorized: return Unauthorized(model);
                case MessageSenderTypes.LoginAgain: return LoginAgain(model);
                case MessageSenderTypes.Authorization: return Authorization(model);
                case MessageSenderTypes.Registration: return Registration(model);

                default: return false;
            }
        }

        private bool Unauthorized(EmailModel model)
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            string Body = File.ReadAllText(Path.Combine(currentDirectory, "MessageSender\\Answers\\Unauthorized.html"));

            model.Body = Body;
            model.Subject = "They are trying to hack your account";

            _email.Send(model);
            return true;
        }

        private bool LoginAgain(EmailModel model) 
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            string Body = File.ReadAllText(Path.Combine(currentDirectory, "MessageSender\\Answers\\LoginAgain.html"));
            
            model.Body = Body;
            model.Subject = "Unauthorized Access Attempt";

            _email.Send(model);
            return true;
        }
        private bool Authorization(EmailModel model)
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            string Body = File.ReadAllText(Path.Combine(currentDirectory, "MessageSender\\Answers\\Authorization.html"));

            model.Body = Body;
            model.Subject = "Authorization";

            _email.Send(model);
            return true;
        }
        private bool Registration(EmailModel model)
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            string Body = File.ReadAllText(Path.Combine(currentDirectory, "MessageSender\\Answers\\Registration.html"));

            model.Body = Body;
            model.Subject = "Registration";

            _email.Send(model);
            return true;
        }

    }

}
