using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ogma3.Data;
using Ogma3.Data.CommentsThreads;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Infrastructure.OgmaConfig;
using Ogma3.Infrastructure.ServiceRegistrations;
using Routes.Areas.Admin.Pages;
using Utils.Extensions;

namespace Ogma3.Areas.Admin.Pages.News;

[Authorize(AuthorizationPolicies.RequireAdminOrModeratorRole)]
public sealed class CreateModel(AppDbContext ctx, OgmaConfig config) : PageModel
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

		public required bool Published { get; set; }
	}

	public IActionResult OnGet()
	{
		return Page();
	}

	public async Task<IActionResult> OnPostAsync()
	{
		if (User.GetNumericId() is not {} uid)
		{
			return Unauthorized();
		}

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

		var news = new Data.NewsPosts.News
		{
			Title = Input.Title,
			Body = Input.Body,
			Slug = Input.Title.Friendlify()
				.ToLowerInvariant(),
			ExcerptCutoff = cutoff,
			IsVisible = Input.Published,
			PublicationDate = Input.Published
				? DateTimeOffset.UtcNow
				: null,
			AuthorId = uid,
			CommentThread = new CommentThread(),
		};

		ctx.News.Add(news);
		await ctx.SaveChangesAsync();

		return News_Index.Get().Redirect(this);
	}
}