using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Blogposts;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Services.DeletionTokenService;
using Utils.Extensions;

namespace Ogma3.Pages.Blog;

[Authorize]
public sealed class RestoreModel(AppDbContext context, IDeletionTokenService tokenService) : PageModel
{
	[BindProperty(SupportsGet = true)]
	public required string Token { get; set; }

	public string? ErrorMessage { get; set; }
	public string? SuccessMessage { get; set; }
	public string? ContentTitle { get; set; }
	public long? RestoredBlogpostId { get; set; }

	public async Task<IActionResult> OnGetAsync()
	{
		await Resolve();
		return Page();
	}

	public async Task<IActionResult> OnPostAsync()
	{
		if (await Resolve() is not { } blogpost)
		{
			return Page();
		}

		blogpost.ScheduledForDeletion = null;
		await context.SaveChangesAsync();

		SuccessMessage = $"""Blog post "{blogpost.Title}" has been restored successfully.""";
		RestoredBlogpostId = blogpost.Id;

		return Page();
	}

	/// <summary>
	/// Validates the token and resolves the blog post it points at, or populates <see cref="ErrorMessage"/>.
	/// </summary>
	private async Task<Blogpost?> Resolve()
	{
		if (!tokenService.TryParseToken(Token, out var contentId, out var scheduledFor, out var contentType))
		{
			ErrorMessage = "Invalid or expired restore token.";
			return null;
		}

		if (contentType != "blogpost")
		{
			ErrorMessage = "Invalid token for this content type.";
			return null;
		}

		if (User.GetNumericId() is not { } uid)
		{
			ErrorMessage = "You need to be signed in to restore this content.";
			return null;
		}

		var blogpost = await context.Blogposts
			.IgnoreQueryFilters()
			.Where(b => b.Id == contentId)
			.Where(b => b.AuthorId == uid)
			.FirstOrDefaultAsync();

		if (blogpost is null)
		{
			ErrorMessage = "Blog post not found or you don't have permission to restore it.";
			return null;
		}

		if (blogpost.ScheduledForDeletion is not {} blogpostSchedule)
		{
			ErrorMessage = "This restore link has expired or is invalid.";
			return null;
		}

		// Verify scheduled time matches
		if (!blogpostSchedule.MicrosecondEqual(scheduledFor))
		{
			ErrorMessage = "This restore link has expired or is invalid.";
			return null;
		}

		ContentTitle = blogpost.Title;
		return blogpost;
	}
}
