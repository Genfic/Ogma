using Immediate.Injections.Shared;
using Microsoft.Extensions.Options;
using PostmarkDotNet;
using Utils.Extensions;

namespace Ogma3.Services.Mailer;

[RegisterTransient<IMailer>]
public sealed class PostmarkMailer(IOptions<PostmarkOptions> options, ILogger<PostmarkMailer> logger) : IMailer
{
	private readonly PostmarkOptions _options = options.Value;
	private readonly PostmarkClient _client = new(options.Value.Key);

	public async Task SendEmailAsync(string email, string subject, string htmlMessage)
	{
		var message = new PostmarkMessage
		{
			To = email,
			From = $"admin@{_options.Domain}",
			TrackOpens = true,
			Subject = subject,
			HtmlBody = htmlMessage,
			MessageStream = "outbound",
		};
		var result = await _client.SendMessageAsync(message);

		LogResult(result);
	}

	public async Task SendEmailTemplateAsync(string email, string templateName, Dictionary<string, string> model)
	{
		var message = new TemplatedPostmarkMessage
		{
			To = email,
			From = $"admin@{_options.Domain}",
			TrackOpens = true,
			TemplateAlias = templateName,
			TemplateModel = model,
			MessageStream = "outbound",
		};
		var result = await _client.SendMessageAsync(message);

		LogResult(result);
	}

	private void LogResult(PostmarkResponse? result)
	{
		if (result is null)
		{
			return;
		}
		if (result.Status == PostmarkStatus.Success)
		{
			logger.LogInformation("Email {EmailId} sent to {Email} at {Time}", result.MessageID, ObfuscateEmail(result.To), result.SubmittedAt);
		}
		else
		{
			logger.LogError(
				"Postmark email sending error. Status: {Status}, Message: {Message}, Error: {ErrorCode}",
				result.Status, result.Message, result.ErrorCode
			);
		}
	}

	private static string ObfuscateEmail(ReadOnlySpan<char> email)
	{
		var at = email.IndexOf('@');
		var start = email[..at].Obfuscate();
		var end = email[(at + 1)..];
		return $"{start}@{end}";
	}
}