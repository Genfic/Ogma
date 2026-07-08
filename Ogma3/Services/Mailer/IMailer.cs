using Microsoft.AspNetCore.Identity.UI.Services;

namespace Ogma3.Services.Mailer;

public interface IMailer : IEmailSender
{
	Task SendEmailTemplateAsync(string email, string templateName, Dictionary<string, string> model);
}