using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Chapters;
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
	public string? ContentTitle { get; set; }
	public long? RestoredChapterId { get; set; }
	public long? RestoredStoryId { get; set; }

	public async Task<IActionResult> OnGetAsync()
	{
		await Resolve();
		return Page();
	}

	public async Task<IActionResult> OnPostAsync()
	{
		if (await Resolve() is not { } chapter)
		{
			return Page();
		}

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

	/// <summary>
	/// Validates the token and resolves the chapter it points at, or populates <see cref="ErrorMessage"/>.
	/// </summary>
	private async Task<Chapter?> Resolve()
	{
		if (!tokenService.TryParseToken(Token, out var contentId, out var scheduledFor, out var contentType))
		{
			ErrorMessage = "Invalid or expired restore token.";
			return null;
		}

		if (contentType != "chapter")
		{
			ErrorMessage = "Invalid token for this content type.";
			return null;
		}

		if (User.GetNumericId() is not { } uid)
		{
			ErrorMessage = "You need to be signed in to restore this content.";
			return null;
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
			return null;
		}

		if (chapter.ScheduledForDeletion is not {} chapterSchedule)
		{
			ErrorMessage = "This restore link has expired or is invalid.";
			return null;
		}

		// Verify scheduled time matches
		if (!chapterSchedule.MicrosecondEqual(scheduledFor))
		{
			ErrorMessage = "This restore link has expired or is invalid.";
			return null;
		}

		ContentTitle = chapter.Title;
		return chapter;
	}
}
