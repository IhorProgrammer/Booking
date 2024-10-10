using BookingLibrary.Services.MessageSender;
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


        public bool SendByType(EmailModel model, string typeName ) 
        {
            switch (typeName.ToLower()) 
            { 
                case "hacker": return Unauthorized(model);
                case "loginagain": return LoginAgain(model);
                case "authorization": return Authorization(model);
                case "registration": return Registration(model);
                case "unauthorized": return Unauthorized(model);

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
