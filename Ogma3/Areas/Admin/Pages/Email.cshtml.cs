using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Markdig;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ogma3.Infrastructure.Constants;
using Serilog;

namespace Ogma3.Areas.Admin.Pages;

public class Email(IEmailSender emailSender) : PageModel
{
	public class EmailModel
	{
		[EmailAddress]
		public required string To { get; init; }
		public required string Subject { get; init; }
		public required string Body { get; init; }
		public bool Markdown { get; init; }
	}

	[BindProperty]
	public required EmailModel Mail { get; init; }

	public void OnGet()
	{
	}

	public async Task OnPostAsync()
	{
		var body = Mail.Markdown 
			? Markdown.ToHtml(Mail.Body, MarkdownPipelines.All) 
			: Mail.Body;
		
		Log.Information("✉ Sending email to {Recipient}", Mail.To);
		
		await emailSender.SendEmailAsync(Mail.To, Mail.Subject, body);
	}
}