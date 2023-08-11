using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace Ogma3.Services.Mailer;

public interface IMailer : IEmailSender
{
	Task SendEmailWithAttachmentsAsync(string email, string subject, string htmlMessage);
}