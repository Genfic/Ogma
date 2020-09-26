using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using Ogma3.Data;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Ogma3.Services.Mailer
{
    public class EmailSender : IEmailSender
    {

        private readonly AuthMessageSenderOptions _options; //set only via Secret Manager
        private readonly OgmaConfig _config;
        
        public EmailSender(IOptions<AuthMessageSenderOptions> optionsAccessor, OgmaConfig config)
        {
            _config = config;
            _options = optionsAccessor.Value;
        }

        public Task SendEmailAsync(string email, string subject, string message)
        {
            return Execute(_options.SendGridKey, subject, message, email);
        }

        public Task Execute(string apiKey, string subject, string message, string email)
        {
            var client = new SendGridClient(apiKey);
            var msg = new SendGridMessage
            {
                From = new EmailAddress(_config.AdminEmail, _options.SendGridUser),
                Subject = subject,
                PlainTextContent = message,
                HtmlContent = message
            };
            msg.AddTo(new EmailAddress(email));

            // Disable click tracking.
            // See https://sendgrid.com/docs/User_Guide/Settings/tracking.html
            msg.SetClickTracking(false, false);

            return client.SendEmailAsync(msg);
        }
    }
}