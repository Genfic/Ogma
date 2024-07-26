using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using Ogma3.Data;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Ogma3.Services.Mailer;

public class SendGridMailer : IEmailSender
{
	private readonly SendGridOptions _options;
	private readonly OgmaConfig _config;

	public SendGridMailer(IOptions<SendGridOptions> optionsAccessor, OgmaConfig config)
	{
		_config = config;
		_options = optionsAccessor.Value;
	}

	public Task SendEmailAsync(string email, string subject, string message)
	{
		var client = new SendGridClient(_options.SendGridKey);
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