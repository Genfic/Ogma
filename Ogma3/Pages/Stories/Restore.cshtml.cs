using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Services.DeletionTokenService;
using Utils.Extensions;

namespace Ogma3.Pages.Stories;

[Authorize]
public sealed class RestoreModel(AppDbContext context, IDeletionTokenService tokenService) : PageModel
{
	[BindProperty(SupportsGet = true)]
	public required string Token { get; set; }

	public string? ErrorMessage { get; set; }
	public string? SuccessMessage { get; set; }
	public long? RestoredStoryId { get; set; }

	public async Task<IActionResult> OnGetAsync()
	{
		if (!tokenService.TryParseToken(Token, out var contentId, out var scheduledFor, out var contentType))
		{
			ErrorMessage = "Invalid or expired restore token.";
			return Page();
		}

		if (contentType != "story")
		{
			ErrorMessage = "Invalid token for this content type.";
			return Page();
		}

		if (User.GetNumericId() is not { } uid)
		{
			return Unauthorized();
		}

		var story = await context.Stories
			.IgnoreQueryFilters()
			.Where(s => s.Id == contentId)
			.Where(s => s.AuthorId == uid)
			.FirstOrDefaultAsync();

		if (story is null)
		{
			ErrorMessage = "Story not found or you don't have permission to restore it.";
			return Page();
		}

		if (story.ScheduledForDeletion is not {} storySchedule)
		{
			ErrorMessage = "This restore link has expired or is invalid.";
			return Page();
		}

		// Verify scheduled time matches
		if (!storySchedule.MicrosecondEqual(scheduledFor))
		{
			ErrorMessage = "This restore link has expired or is invalid.";
			return Page();
		}

		// Restore the story
		story.ScheduledForDeletion = null;
		await context.SaveChangesAsync();

		SuccessMessage = $"""Story "{story.Title}" has been restored successfully.""";
		RestoredStoryId = story.Id;

		return Page();
	}
}