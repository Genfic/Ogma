using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Infrastructure.Exceptions;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Services.DeletionTokenService;
using Ogma3.Services.Mailer;
using Ogma3.Services.SafetyPinService;
using Routes.Pages;

namespace Ogma3.Pages.Chapters;

[Authorize]
public sealed class DeleteModel(AppDbContext context, SafetyPinService pinService, IMailer mailer, IDeletionTokenService tokenService) : PageModel
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
		if (User.GetEmail() is not {} email) return Unauthorized();

		HasPin = await pinService.HasPin(uid);

		if (HasPin)
		{
			var msg = await pinService.VerifyPin(uid, Pin) switch
			{
				PinVerificationResult.Invalid => "Incorrect PIN",
				PinVerificationResult.LockedOut => "PIN recently changed, lockout",
				PinVerificationResult.NoPin => "No PIN set",
				PinVerificationResult.NotFound => "User not found",
				PinVerificationResult.NotProvided => "PIN required",
				PinVerificationResult.Valid => null,
				var r => throw new UnexpectedEnumValueException<PinVerificationResult>(r),
			};

			if (msg is not null)
			{
				ModelState.AddModelError("Pin", msg);
				return Page();
			}
		}

		// Get chapter
		var chapter = await context.Chapters
			.Where(c => c.Id == id)
			.Where(c => c.Story.AuthorId == User.GetNumericId())
			.Include(c => c.Story)
			.ThenInclude(s => s.Author)
			.FirstOrDefaultAsync();

		if (chapter is null) return NotFound();

		// Schedule for deletion in 7 days
		var scheduledFor = DateTimeOffset.UtcNow.AddDays(7);
		chapter.ScheduledForDeletion = scheduledFor;

		var undoToken = tokenService.GenerateToken(chapter.Id, scheduledFor, "chapter");

		// Recalculate words and chapters in the story
		chapter.Story.WordCount -= chapter.WordCount;
		chapter.Story.ChapterCount -= 1;

		// Send email with undo link
		var undoUrl = Chapters_Restore.Get(undoToken).Url(Url, Request.Scheme);
		await mailer.SendEmailTemplateAsync(email, "content-scheduled-for-deletion", new()
		{
			["name"] = chapter.Story.Author.UserName,
			["content_title"] = $"{chapter.Story.Title}: {chapter.Title}",
			["content_type"] = "chapter",
			["scheduled_for"] = scheduledFor.ToString("yyyy-MM-dd HH:mm UTC"),
			["undo_url"] = undoUrl ?? "",
		});

		await context.SaveChangesAsync();

		return Story.Get(chapter.StoryId, null).Redirect(this);
	}
}