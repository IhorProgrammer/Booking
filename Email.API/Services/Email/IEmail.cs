using Email.API.Models;

namespace Email.API.Services.Email
{
    public interface IEmail
    {
        public bool Send(EmailModel model);

    }
}
