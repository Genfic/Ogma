using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Stories;
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
	public string? ContentTitle { get; set; }
	public long? RestoredStoryId { get; set; }

	public async Task<IActionResult> OnGetAsync()
	{
		await Resolve();
		return Page();
	}

	public async Task<IActionResult> OnPostAsync()
	{
		if (await Resolve() is not { } story)
		{
			return Page();
		}

		story.ScheduledForDeletion = null;
		await context.SaveChangesAsync();

		SuccessMessage = $"""Story "{story.Title}" has been restored successfully.""";
		RestoredStoryId = story.Id;

		return Page();
	}

	/// <summary>
	/// Validates the token and resolves the story it points at, or populates <see cref="ErrorMessage"/>.
	/// </summary>
	private async Task<Story?> Resolve()
	{
		if (!tokenService.TryParseToken(Token, out var contentId, out var scheduledFor, out var contentType))
		{
			ErrorMessage = "Invalid or expired restore token.";
			return null;
		}

		if (contentType != "story")
		{
			ErrorMessage = "Invalid token for this content type.";
			return null;
		}

		if (User.GetNumericId() is not { } uid)
		{
			ErrorMessage = "You need to be signed in to restore this content.";
			return null;
		}

		var story = await context.Stories
			.IgnoreQueryFilters()
			.Where(s => s.Id == contentId)
			.Where(s => s.AuthorId == uid)
			.FirstOrDefaultAsync();

		if (story is null)
		{
			ErrorMessage = "Story not found or you don't have permission to restore it.";
			return null;
		}

		if (story.ScheduledForDeletion is not {} storySchedule)
		{
			ErrorMessage = "This restore link has expired or is invalid.";
			return null;
		}

		// Verify scheduled time matches
		if (!storySchedule.MicrosecondEqual(scheduledFor))
		{
			ErrorMessage = "This restore link has expired or is invalid.";
			return null;
		}

		ContentTitle = story.Title;
		return story;
	}
}
