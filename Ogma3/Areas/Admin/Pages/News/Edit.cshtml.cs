using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Infrastructure.OgmaConfig;
using Ogma3.Infrastructure.ServiceRegistrations;
using Routes.Areas.Admin.Pages;
using Utils.Extensions;

namespace Ogma3.Areas.Admin.Pages.News;

[Authorize(AuthorizationPolicies.RequireAdminRole)]
public sealed class EditModel(AppDbContext ctx, OgmaConfig config) : PageModel
{
	[BindProperty] public required InputModel Input { get; set; }

	public sealed class InputModel
	{
		public required long Id { get; init; }
		[Required]
		[MinLength(1)]
		public required string Title { get; set; }
		[Required]
		[MinLength(100)]
		public required string Body { get; set; }

		public required bool Published { get; init; }
	}

	public async Task<IActionResult> OnGet(long id)
	{
		var news = await ctx.News
			.Where(n => n.Id == id)
			.Select(n => new InputModel
			{
				Id = n.Id,
				Title = n.Title,
				Body = n.Body,
				Published = n.IsVisible,
			})
			.FirstOrDefaultAsync();

		if (news is null) return NotFound();

		Input = news;

		return Page();
	}

	public async Task<IActionResult> OnPostAsync()
	{
		if (!ModelState.IsValid)
		{
			return Page();
		}

		var body = Input.Body.AsSpan().Trim();
		var cutoff = body.IndexOf(CTConfig.Blogpost.CutoffMarker, StringComparison.OrdinalIgnoreCase);

		if (cutoff <= 0)
		{
			cutoff = body.IndexOfBefore(' ', config.BlogpostExcerptDefaultCutoff);
		}

		_ = await ctx.News
			.Where(n => n.Id == Input.Id)
			.ExecuteUpdateAsync(setters => setters
				.SetProperty(n => n.Title, Input.Title)
				.SetProperty(n => n.Slug, Input.Title.Friendlify().ToLowerInvariant())
				.SetProperty(n => n.Body, Input.Body)
				.SetProperty(n => n.ExcerptCutoff, cutoff)
				.SetProperty(n => n.IsVisible, Input.Published)
				.SetProperty(n => n.PublicationDate, n => n.PublicationDate == null ? DateTimeOffset.UtcNow : n.PublicationDate));

		return News_Index.Get().Redirect(this);
	}
}