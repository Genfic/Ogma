using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using PostmarkDotNet;
using Serilog;

namespace Ogma3.Services.Mailer;

public class PostmarkMailer : IEmailSender
{
	private readonly PostmarkOptions _options;

	public PostmarkMailer(IOptions<PostmarkOptions> options) => _options = options.Value;

	public async Task SendEmailAsync(string email, string subject, string htmlMessage)
	{
		var message = new PostmarkMessage
		{
			To = email,
			From = $"Genfic <noreply@{_options.PostmarkDomain}>",
			TrackOpens = true,
			TrackLinks = LinkTrackingOptions.HtmlAndText,
			Subject = subject,
			HtmlBody = htmlMessage
		};

		var client = new PostmarkClient(_options.PostmarkKey);
		var result = await client.SendMessageAsync(message);

		if (result.Status != PostmarkStatus.Success)
		{
			Log.Error(
				"Postmark email sending error.\n\tStatus: {Status}\n\tMessage: {Message}\n\tError {ErrorCode}",
				result.Status, result.Message, result.ErrorCode
			);
		}
	}
}