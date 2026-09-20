using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Services.DeletionTokenService;
using Utils.Extensions;

namespace Ogma3.Pages.Chapters;

[Authorize]
public sealed class RestoreModel(AppDbContext context, IDeletionTokenService tokenService) : PageModel
{
	[BindProperty(SupportsGet = true)]
	public required string Token { get; set; }

	public string? ErrorMessage { get; set; }
	public string? SuccessMessage { get; set; }
	public long? RestoredChapterId { get; set; }
	public long? RestoredStoryId { get; set; }

	public async Task<IActionResult> OnGetAsync()
	{
		if (!tokenService.TryParseToken(Token, out var contentId, out var scheduledFor, out var contentType))
		{
			ErrorMessage = "Invalid or expired restore token.";
			return Page();
		}

		if (contentType != "chapter")
		{
			ErrorMessage = "Invalid token for this content type.";
			return Page();
		}

		if (User.GetNumericId() is not { } uid)
		{
			return Unauthorized();
		}

		var chapter = await context.Chapters
			.IgnoreQueryFilters()
			.Where(c => c.Id == contentId)
			.Where(c => c.Story.AuthorId == uid)
			.Include(c => c.Story)
			.FirstOrDefaultAsync();

		if (chapter is null)
		{
			ErrorMessage = "Chapter not found or you don't have permission to restore it.";
			return Page();
		}

		if (chapter.ScheduledForDeletion is not {} chapterSchedule)
		{
			ErrorMessage = "This restore link has expired or is invalid.";
			return Page();
		}

		// Verify scheduled time matches
		if (!chapterSchedule.MicrosecondEqual(scheduledFor))
		{
			ErrorMessage = "This restore link has expired or is invalid.";
			return Page();
		}

		// Restore the chapter
		chapter.ScheduledForDeletion = null;

		// Recalculate words and chapters in the story
		chapter.Story.WordCount += chapter.WordCount;
		chapter.Story.ChapterCount += 1;

		await context.SaveChangesAsync();

		SuccessMessage = $"""Chapter "{chapter.Title}" has been restored successfully.""";
		RestoredChapterId = chapter.Id;
		RestoredStoryId = chapter.StoryId;

		return Page();
	}
}