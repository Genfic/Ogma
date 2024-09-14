using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using Serilog;

namespace Ogma3.Services.Mailer;

public sealed class MailGunMailer(IHttpClientFactory httpClientFactory, IOptions<MailGunOptions> options)
	: IEmailSender
{
	private readonly MailGunOptions _options = options.Value;

	public async Task SendEmailAsync(string email, string subject, string htmlMessage)
	{
		var client = httpClientFactory.CreateClient();
		var auth = Convert.ToBase64String(Encoding.ASCII.GetBytes($"api:{_options.MailGunKey}"));
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", auth);

		var content = new FormUrlEncodedContent(new Dictionary<string, string>
		{
			["from"] = $"Genfic <noreply@{_options.MailGunDomain}>",
			["to"] = email,
			["subject"] = subject,
			["html"] = htmlMessage,
		});

		var response = await client.PostAsync($"https://api.eu.mailgun.net/v3/{_options.MailGunDomain}/messages", content);

		if (!response.IsSuccessStatusCode)
		{
			var msg = await response.Content.ReadAsStringAsync();
			Log.Error("Error trying to send email to [{To}], with subject [{Subject}].\n | Status: [{Status}]\n | Message: [{Msg}]", email,
				subject, response.StatusCode, msg);
		}
	}
}