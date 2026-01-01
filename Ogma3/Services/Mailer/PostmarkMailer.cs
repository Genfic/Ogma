using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using PostmarkDotNet;

namespace Ogma3.Services.Mailer;

public sealed class PostmarkMailer(IOptions<PostmarkOptions> options, ILogger<PostmarkMailer> logger ) : IEmailSender
{
	private readonly PostmarkOptions _options = options.Value;

	public async Task SendEmailAsync(string email, string subject, string htmlMessage)
	{
		var message = new PostmarkMessage
		{
			To = email,
			From = $"admin@{_options.Domain}",
			TrackOpens = true,
			TrackLinks = LinkTrackingOptions.HtmlAndText,
			Subject = subject,
			HtmlBody = htmlMessage,
			MessageStream = "outbound",
		};

		var client = new PostmarkClient(_options.Key);
		var result = await client.SendMessageAsync(message);

		if (result.Status == PostmarkStatus.Success)
		{
			logger.LogInformation("Email {EmailId} sent to {Email} at {Time}", result.MessageID, email, result.SubmittedAt);
		}
		else
		{
			logger.LogError("""
			                Postmark email sending error.
			                	Status: {Status}
			                	Message: {Message}
			                	Error {ErrorCode}",
			                """,
				result.Status, result.Message, result.ErrorCode
			);
		}
	}
}