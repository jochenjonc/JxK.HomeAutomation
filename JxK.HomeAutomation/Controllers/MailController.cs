using System.Net;
using System.Net.Mail;

namespace JxK.HomeAutomation.Controllers
{
    internal class MailController
    {
        private SmtpClient _smtpClient;

        public MailController(string smtpHost, int smtpPort, string username, string password)
        {
            _smtpClient = new SmtpClient(smtpHost, smtpPort);
            _smtpClient.UseDefaultCredentials = false;
            _smtpClient.Credentials = new NetworkCredential(username, password);
            _smtpClient.EnableSsl = true;
        }

        public void Send(string to, string subject, string body)
        {
            var mailMessage = new MailMessage
            {
                From = new MailAddress("automation@jxk.be")
            };
            mailMessage.To.Add(to);
            mailMessage.Subject = subject;
            mailMessage.Body = body;

            _smtpClient.Send(mailMessage);
        }
    }
}
