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
using Ogma3.Services.ModeratorActionService;
using Routes.Areas.Admin.Pages;
using Utils.Extensions;

namespace Ogma3.Areas.Admin.Pages.Documents;

[Authorize(AuthorizationPolicies.RequireAdminRole)]
public sealed class CreateModel(AppDbContext context, IModeratorActionService moderatorActionService) : PageModel
{
	[BindProperty] public required InputModel Input { get; set; }

	public sealed class InputModel
	{
		[Required]
		[MinLength(1)]
		public required string Title { get; set; }
		[Required]
		[MinLength(100)]
		public required string Body { get; set; }
		public string? CustomCss { get; set; }
		public string? CustomJs { get; set; }
	}

	public IActionResult OnGet()
	{
		return Page();
	}

	public async Task<IActionResult> OnPostAsync()
	{
		if (!ModelState.IsValid)
		{
			return Page();
		}

		var document = Markdown.Parse(Input.Body, MarkdownPipelines.AllWithHtml);

		var toc = document
			.Descendants<HeadingBlock>()
			.Select(h => new Document.Header((byte)h.Level, h.GetAttributes().Id ?? "", h.Inline?
				.Descendants<LiteralInline>()
				.Aggregate("", (acc, l) => acc + l.Content) ?? "")
			)
			.ToList();

		var admin = User.IsInRole(RoleNames.Admin);

		var doc = new Document
		{
			Title = Input.Title,
			Slug = Input.Title.Friendlify(),
			Body = Input.Body,
			CompiledBody = document.ToHtml(),
			CustomCss = Input.CustomCss,
			CustomJs = admin ? Input.CustomJs : null,
			Version = 1,
			CreationTime = DateTimeOffset.UtcNow,
			RevisionDate = null,
			Headers = toc,
		};

		context.Documents.Add(doc);

		await context.SaveChangesAsync();

		moderatorActionService.LogDocumentCreated(doc.Slug, doc.Title);

		return Documents_Index.Get().Redirect(this);
	}
}
