using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;

namespace JxK.HomeAutomation.Controllers
{
    internal class MailController
    {
        private ResourceLoader _resourceLoader;

        private string _smtpHost;
        private int _smtpPort;
        private string _username;
        private string _password;

        private string _fromMailAddress;
        private string _toMailAddress;

        public MailController()
        {
            _resourceLoader = ResourceLoader.GetForViewIndependentUse("MailResources");

            _smtpHost = _resourceLoader.GetString("SmtpHost");
            _smtpPort = int.Parse(_resourceLoader.GetString("SmtpPort"));
            _username = _resourceLoader.GetString("SmtpUsername");
            _password = _resourceLoader.GetString("SmtpPassword");

            _fromMailAddress = _resourceLoader.GetString("FromMailAddress");
            _toMailAddress = _resourceLoader.GetString("ToMailAddress");
        }

        public string SubjectPrefix { get; set; }

        public async Task Send(string subject, string body)
        {
            using (var smtpClient = new SmtpClient(_smtpHost, _smtpPort))
            {
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(_username, _password);
                smtpClient.EnableSsl = true;

                if (!string.IsNullOrEmpty(SubjectPrefix))
                {
                    subject = $"{SubjectPrefix} {subject}";
                }

                using (var mailMessage = new MailMessage())
                {
                    mailMessage.From = new MailAddress(_fromMailAddress);
                    mailMessage.To.Add(_toMailAddress);

                    mailMessage.Subject = subject;
                    mailMessage.Body = body;

                    await smtpClient.SendMailAsync(mailMessage);
                }
            }
        }
    }
}