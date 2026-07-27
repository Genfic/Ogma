using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Services.SafetyPinService;

namespace Ogma3.Pages.Blog;

[Authorize]
public sealed class DeleteModel(ApplicationDbContext context, SafetyPinService pinService) : PageModel
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

		HasPin = await pinService.HasPin(uid);

		var check = Pin is not null && await pinService.VerifyPin(uid, Pin);
		if (!check)
		{
			ModelState.AddModelError("Pin", "Incorrect PIN");
			return Page();
		}

		var rows = await context.Blogposts
			.Where(b => b.Id == id)
			.Where(b => b.AuthorId == uid)
			.ExecuteDeleteAsync();

		if (rows <= 0) return NotFound();

		return Routes.Pages.User_Blog.Get(uname).Redirect(this);
	}
}