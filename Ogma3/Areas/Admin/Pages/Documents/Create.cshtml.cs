using System.ComponentModel.DataAnnotations;
using Markdig;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ogma3.Data;
using Ogma3.Data.Documents;
using Ogma3.Infrastructure.Constants;
using Utils.Extensions;

namespace Ogma3.Areas.Admin.Pages.Documents;

public sealed class CreateModel(ApplicationDbContext context) : PageModel
{
	[BindProperty] public required InputModel Input { get; set; }

	public sealed class InputModel
	{
		public required long Id { get; set; }

		[Required] public required string Title { get; set; }

		[Required] public required string Body { get; set; }
	}

	public IActionResult OnGet()
	{
		return Page();
	}

	public async Task<IActionResult> OnPostAsync()
	{
		var html = Markdown.ToHtml(Input.Body, MarkdownPipelines.All);

		context.Documents.Add(new Document
		{
			Title = Input.Title,
			Slug = Input.Title.Friendlify(),
			Body = Input.Body,
			CompiledBody = html,
			Version = 1,
			CreationTime = DateTimeOffset.UtcNow,
			RevisionDate = null,
			Headers = Input.Body.GetMarkdownHeaders()
				.Select(h => new Document.Header(h.Level, h.Occurrence, h.Body))
				.ToList(),
		});

		await context.SaveChangesAsync();
		return Routes.Pages.Index.Get().Redirect(this);
	}
}