using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Infrastructure.Exceptions;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Services.SafetyPinService;

namespace Ogma3.Pages.Chapters;

[Authorize]
public sealed class DeleteModel(AppDbContext context, SafetyPinService pinService) : PageModel
{
	[BindProperty]
	public required GetData Chapter { get; set; }

	public sealed class GetData
	{
		public required long Id { get; init; }
		public required DateTimeOffset? PublishDate { get; init; }
		public required string Title { get; init; }
		public required string Slug { get; init; }
		public required int WordCount { get; init; }
		public required int CommentsThreadCommentsCount { get; init; }
		public required string StoryTitle { get; init; }
		public required long StoryId { get; init; }
	}

	public required bool HasPin { get; set; }
	[BindProperty]
	public string? Pin { get; set; }

	public async Task<IActionResult> OnGetAsync(long id)
	{
		if (User.GetNumericId() is not {} uid)
		{
			return Unauthorized();
		}

		HasPin = await pinService.HasPin(uid);

		var chapter = await context.Chapters
			.Where(c => c.Id == id)
			.Where(c => c.Story.AuthorId == uid)
			.Select(c => new GetData
			{
				Id = c.Id,
				Title = c.Title,
				Slug = c.Slug,
				StoryId = c.StoryId,
				StoryTitle = c.Story.Title,
				WordCount = c.WordCount,
				PublishDate = c.PublicationDate,
				CommentsThreadCommentsCount = c.CommentThread.CommentsCount,
			})
			.FirstOrDefaultAsync();

		if (chapter is null) return NotFound();

		Chapter = chapter;

		return Page();
	}

	public async Task<IActionResult> OnPostAsync(long id)
	{
		if (User.GetNumericId() is not {} uid)
		{
			return Unauthorized();
		}

		HasPin = await pinService.HasPin(uid);

		if (HasPin)
		{
			if (Pin is not {} pin)
			{
				ModelState.AddModelError("Pin", "Pin required");
				return Page();
			}

			var res = await pinService.VerifyPin(uid, pin);
			if (res != PinVerificationResult.Valid)
			{
				var msg = res switch
				{
					PinVerificationResult.Invalid => "Incorrect PIN",
					PinVerificationResult.LockedOut => "PIN recently changed, lockout",
					PinVerificationResult.NoPin => "No PIN set",
					PinVerificationResult.NotFound => "User not found",
					_ => throw new UnexpectedEnumValueException<PinVerificationResult>(res),
				};
				ModelState.AddModelError("Pin", msg);
				return Page();
			}
		}

		// Get chapter
		var chapter = await context.Chapters
			.Where(c => c.Id == id)
			.Where(c => c.Story.AuthorId == User.GetNumericId())
			.Include(c => c.Story)
			.FirstOrDefaultAsync();

		if (chapter is null) return NotFound();

		// Recalculate words and chapters in the story
		chapter.Story.WordCount -= chapter.WordCount;
		chapter.Story.ChapterCount -= 1;

		context.Chapters.Remove(chapter);

		await context.SaveChangesAsync();

		return Routes.Pages.Story.Get(chapter.StoryId, null).Redirect(this);
	}
}