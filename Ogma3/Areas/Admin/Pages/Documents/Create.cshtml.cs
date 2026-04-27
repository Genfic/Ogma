using System.ComponentModel.DataAnnotations;
using Markdig;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ogma3.Data;
using Ogma3.Data.Documents;
using Ogma3.Infrastructure.Constants;
using Ogma3.Infrastructure.ServiceRegistrations;
using Routes.Areas.Admin.Pages;
using Utils.Extensions;

namespace Ogma3.Areas.Admin.Pages.Documents;

[Authorize(AuthorizationPolicies.RequireAdminRole)]
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
		var document = Markdown.Parse(Input.Body, MarkdownPipelines.All);

		var toc = document
			.Descendants<HeadingBlock>()
			.Select(h => new Document.Header((byte)h.Level, h.GetAttributes().Id ?? "", h.Inline?
				.Descendants<LiteralInline>()
				.Aggregate("", (acc, l) => acc + l.Content) ?? "")
			)
			.ToList();

		context.Documents.Add(new Document
		{
			Title = Input.Title,
			Slug = Input.Title.Friendlify(),
			Body = Input.Body,
			CompiledBody = document.ToHtml(),
			Version = 1,
			CreationTime = DateTimeOffset.UtcNow,
			RevisionDate = null,
			Headers = toc,
		});

		await context.SaveChangesAsync();
		return Documents_Index.Get().Redirect(this);
	}
}