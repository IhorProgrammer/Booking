using System.Net.Mail;
using System.Net;
using Email.API.Models;

namespace Email.API.Services.Email
{
    public class Email : IEmail
    {
        public bool Send( EmailModel emailModel )
        {
            MailAddress from = new MailAddress(emailModel.From, emailModel.FromName);
            MailAddress to = new MailAddress(emailModel.To, emailModel.ToName);
            MailMessage msg = new MailMessage(from, to);
            msg.Subject = emailModel.Subject;
            msg.Body = emailModel.Body;
            msg.IsBodyHtml = true;
            SmtpClient client = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(emailModel.From, emailModel.FromPassword),
                EnableSsl = true,
            };
            client.Send(msg);
            return true;
        }


    }
}
