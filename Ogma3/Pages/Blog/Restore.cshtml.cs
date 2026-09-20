using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
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

	public async Task<IActionResult> OnGetAsync()
	{
		if (!tokenService.TryParseToken(Token, out var contentId, out var scheduledFor, out var contentType))
		{
			ErrorMessage = "Invalid or expired restore token.";
			return Page();
		}

		if (contentType != "blogpost")
		{
			ErrorMessage = "Invalid token for this content type.";
			return Page();
		}

		if (User.GetNumericId() is not { } uid)
		{
			return Unauthorized();
		}

		var blogpost = await context.Blogposts
			.IgnoreQueryFilters()
			.Where(b => b.Id == contentId)
			.Where(b => b.AuthorId == uid)
			.FirstOrDefaultAsync();

		if (blogpost is null)
		{
			ErrorMessage = "Blog post not found or you don't have permission to restore it.";
			return Page();
		}

		if (blogpost.ScheduledForDeletion is not {} blogpostSchedule)
		{
			ErrorMessage = "This restore link has expired or is invalid.";
			return Page();
		}

		// Verify scheduled time matches
		if (!blogpostSchedule.MicrosecondEqual(scheduledFor))
		{
			ErrorMessage = "This restore link has expired or is invalid.";
			return Page();
		}

		// Restore the blogpost
		blogpost.ScheduledForDeletion = null;
		await context.SaveChangesAsync();

		SuccessMessage = $"""Blog post "{blogpost.Title}" has been restored successfully.""";

		return Page();
	}
}