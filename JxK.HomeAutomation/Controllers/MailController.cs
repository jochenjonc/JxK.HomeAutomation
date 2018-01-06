using System.Net;
using System.Net.Mail;
using Windows.ApplicationModel.Resources;

namespace JxK.HomeAutomation.Controllers
{
    internal class MailController
    {
        private SmtpClient _smtpClient;
        private ResourceLoader _resourceLoader;

        public MailController()
        {
            _resourceLoader = ResourceLoader.GetForViewIndependentUse("MailResources");

            var smtpHost = _resourceLoader.GetString("SmtpHost");
            var smtpPort = int.Parse(_resourceLoader.GetString("SmtpPort"));
            var username = _resourceLoader.GetString("SmtpUsername");
            var password = _resourceLoader.GetString("SmtpPassword");

            _smtpClient = new SmtpClient(smtpHost, smtpPort)
            {
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(username, password),
                EnableSsl = true
            };
        }

        public void Send(string subject, string body)
        {
            var fromMailAddress = _resourceLoader.GetString("FromMailAddress");
            var toMailAddress = _resourceLoader.GetString("ToMailAddress");

            var mailMessage = new MailMessage
            {
                From = new MailAddress(fromMailAddress)
            };
            mailMessage.To.Add(toMailAddress);
            mailMessage.Subject = subject;
            mailMessage.Body = body;

            _smtpClient.Send(mailMessage);
        }
    }
}
