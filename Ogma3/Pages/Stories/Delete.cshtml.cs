using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Stories;
using Ogma3.Infrastructure.Exceptions;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Services.DeletionTokenService;
using Ogma3.Services.Mailer;
using Ogma3.Services.SafetyPinService;
using Routes.Pages;

namespace Ogma3.Pages.Stories;

[Authorize]
public sealed class DeleteModel(AppDbContext context, SafetyPinService pinService, IMailer mailer, IDeletionTokenService tokenService) : PageModel
{
	public sealed class GetData
	{
		public required long Id { get; init; }
		public required string Title { get; init; }
		public required string Slug { get; init; }
		public required DateTimeOffset? ReleaseDate { get; init; }
		public required DateTimeOffset CreationDate { get; init; }
		public required string Hook { get; init; }
		public required bool IsPublished { get; init; }
		public required EStoryStatus Status { get; init; }
		public required int VotesCount { get; init; }
		public required int ChaptersCount { get; init; }
		public required int CommentsCount { get; init; }
	}

	[BindProperty] public required GetData Story { get; set; }

	public required bool HasPin { get; set; }

	[BindProperty]
	public required string? Pin { get; set; }

	public async Task<IActionResult> OnGetAsync(int? id)
	{
		if (id is null) return NotFound();
		if (User.GetNumericId() is not { } uid) return Unauthorized();

		HasPin = await pinService.HasPin(uid);

		// Get the story and make sure the logged-in user matches author
		var story = await context.Stories
			.Where(s => s.Id == id)
			.Where(s => s.AuthorId == uid)
			.Select(s => new GetData
			{
				Id = s.Id,
				Title = s.Title,
				Slug = s.Slug,
				ReleaseDate = s.PublicationDate,
				CreationDate = s.CreationDate,
				Hook = s.Hook,
				IsPublished = s.IsVisible,
				Status = s.Status,
				VotesCount = s.Votes.Count,
				ChaptersCount = s.Chapters.Count,
				CommentsCount = s.Chapters.Sum(c => c.CommentThread.CommentsCount),
			})
			.AsNoTracking()
			.FirstOrDefaultAsync();

		if (story is null) return NotFound();
		Story = story;

		return Page();
	}

	public async Task<IActionResult> OnPostAsync(int? id)
	{
		if (id is null) return NotFound();
		if (User.GetNumericId() is not { } uid) return Unauthorized();
		if (User.GetUsername() is not {} uname) return Unauthorized();
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

		// Get the story and make sure the logged-in user matches author
		var story = await context.Stories
			.Where(s => s.Id == id)
			.Include(s => s.Author)
			.FirstOrDefaultAsync();

		if (story is null) return NotFound();
		if (story.AuthorId != uid) return Unauthorized();

		// Schedule for deletion in 7 days
		var scheduledFor = DateTimeOffset.UtcNow.AddDays(7);
		story.ScheduledForDeletion = scheduledFor;

		var undoToken = tokenService.GenerateToken(story.Id, scheduledFor, "story");

		// Persist before emailing, so the undo link can never point at content that was not
		// actually scheduled.
		await context.SaveChangesAsync();

		// Send email with undo link
		var undoUrl = Stories_Restore.Get(undoToken).Url(Url, Request.Scheme);
		await mailer.SendEmailTemplateAsync(email, "content-scheduled-for-deletion", new()
		{
			["name"] = story.Author.UserName,
			["content_title"] = story.Title,
			["content_type"] = "story",
			["scheduled_for"] = scheduledFor.ToString("yyyy-MM-dd HH:mm UTC"),
			["undo_url"] = undoUrl ?? "",
		});

		return User_Stories.Get(uname).Redirect(this);
	}
}