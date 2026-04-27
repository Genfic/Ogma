using System.ComponentModel.DataAnnotations;
using Markdig;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Documents;
using Ogma3.Infrastructure.Constants;
using Ogma3.Infrastructure.ServiceRegistrations;
using Routes.Areas.Admin.Pages;

namespace Ogma3.Areas.Admin.Pages.Documents;

[Authorize(AuthorizationPolicies.RequireAdminRole)]
public sealed class EditModel(ApplicationDbContext context) : PageModel
{
	[BindProperty] public required InputModel Input { get; set; }

	public sealed class InputModel
	{
		[Required] public required string Slug { get; set; }
		[Required] public required string Title { get; set; }
		[Required] public required string Body { get; set; }
		public uint Version { get; set; }
	}

	public async Task<IActionResult> OnGetAsync(string slug)
	{
		var doc = await context.Documents
			.Where(d => d.Slug == slug)
			.Where(d => d.RevisionDate == null)
			.Select(d => new InputModel
			{
				Slug = d.Slug,
				Title = d.Title,
				Body = d.Body,
				Version = d.Version,
			})
			.FirstOrDefaultAsync();

		if (doc is null) return NotFound();

		Input = doc;

		return Page();
	}

	public async Task<IActionResult> OnPostAsync()
	{
		var oldVersion = await context.Documents
			.Where(d => d.RevisionDate == null)
			.Where(d => d.Slug == Input.Slug)
			.FirstOrDefaultAsync();

		if (oldVersion is null) return Page();

		var now = DateTimeOffset.UtcNow;

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
			Title = oldVersion.Title,
			Slug = oldVersion.Slug,
			Body = Input.Body,
			CompiledBody = document.ToHtml(),
			Version = oldVersion.Version + 1,
			CreationTime = now,
			RevisionDate = null,
			Headers = toc,
		});

		oldVersion.RevisionDate = now;

		await context.SaveChangesAsync();
		return Documents_Index.Get().Redirect(this);
	}
}