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

namespace Ogma3.Pages.Blog;

[Authorize]
public sealed class DeleteModel(AppDbContext context, SafetyPinService pinService, IMailer mailer, IDeletionTokenService tokenService) : PageModel
{
	[BindProperty]
	public required GetData Blogpost { get; set; }

	public sealed class GetData
	{
		public required long Id { get; init; }
		public required long AuthorId { get; init; }
		public required string Title { get; init; }
		public required string Slug { get; init; }
		public DateTimeOffset? PublishDate { get; init; }
		public required int CommentsCount { get; init; }
	}

	public required bool HasPin { get; set; }

	[BindProperty]
	public required string? Pin { get; set; }

	public async Task<IActionResult> OnGetAsync(int? id)
	{
		if (id is null) return NotFound();

		if (User.GetNumericId() is not { } uid) return Unauthorized();

		HasPin = await pinService.HasPin(uid);

		var blogpost = await context.Blogposts
			.Where(m => m.Id == id)
			.Where(m => m.AuthorId == uid)
			.Select(b => new GetData
			{
				Id = b.Id,
				AuthorId = b.AuthorId,
				Title = b.Title,
				Slug = b.Slug,
				PublishDate = b.PublicationDate,
				CommentsCount = b.CommentThread.CommentsCount,
			})
			.FirstOrDefaultAsync();

		if (blogpost is null) return NotFound();

		Blogpost = blogpost;

		return Page();
	}

	public async Task<IActionResult> OnPostAsync(int? id)
	{
		if (id is null) return NotFound();

		// Get logged-in user
		var uname = User.GetUsername();
		if (uname is null) return Unauthorized();
		if (User.GetNumericId() is not { } uid) return Unauthorized();
		if (User.GetEmail() is not {} email) return Unauthorized();

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

		// Get blogpost
		var blogpost = await context.Blogposts
			.Where(b => b.Id == id)
			.Where(b => b.AuthorId == uid)
			.Include(b => b.Author)
			.FirstOrDefaultAsync();

		if (blogpost is null) return NotFound();

		// Schedule for deletion in 7 days
		var scheduledFor = DateTimeOffset.UtcNow.AddDays(7);
		blogpost.ScheduledForDeletion = scheduledFor;

		var undoToken = tokenService.GenerateToken(blogpost.Id, scheduledFor, "blogpost");

		// Send email with undo link
		var undoUrl = Blog_Restore.Get(undoToken).Url(Url, Request.Scheme);
		await mailer.SendEmailTemplateAsync(email, "content-scheduled-for-deletion", new()
		{
			["name"] = blogpost.Author.UserName,
			["content_title"] = blogpost.Title,
			["content_type"] = "blogpost",
			["scheduled_for"] = scheduledFor.ToString("yyyy-MM-dd HH:mm UTC"),
			["undo_url"] = undoUrl ?? "",
		});

		await context.SaveChangesAsync();

		return User_Blog.Get(uname).Redirect(this);
	}
}