using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Markdig;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ogma3.Infrastructure.Constants;

namespace Ogma3.Areas.Admin.Pages;

public class Email : PageModel
{
	private readonly IEmailSender _emailSender;
	public Email(IEmailSender emailSender) => _emailSender = emailSender;

	public class EmailModel
	{
		[EmailAddress] public string To { get; init; }
		public string Subject { get; init; }
		public string Body { get; init; }
		public bool Markdown { get; init; }
	}

	[BindProperty] public EmailModel Mail { get; set; }

	public void OnGet()
	{
	}

	public async Task OnPostAsync()
	{
		var body = Mail.Markdown 
			? Markdown.ToHtml(Mail.Body, MarkdownPipelines.All) 
			: Mail.Body;
		
		await _emailSender.SendEmailAsync(Mail.To, Mail.Subject, body);
	}
}