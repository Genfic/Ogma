using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Clubs;
using Ogma3.Data.Comments;
using Ogma3.Data.Roles;
using Ogma3.Pages.Shared;
using Ogma3.Pages.Shared.Bars;

namespace Ogma3.Pages.Club.Forums;

public sealed class DetailsModel(ClubRepository clubRepo, ApplicationDbContext context) : PageModel
{
	public sealed class ThreadDetails
	{
		public required long Id { get; init; }
		public required long ClubId { get; init; }
		public required string Title { get; init; }
		public required string Body { get; init; }
		public required bool IsPinned { get; init; }
		public required DateTimeOffset CreationDate { get; init; }
		public required string AuthorName { get; init; }
		public required long AuthorId { get; init; }
		public required string AuthorAvatar { get; init; }
		public OgmaRole? AuthorRole { get; init; }
		public required CommentsThreadDto CommentsThread { get; init; }
	}

	public required ThreadDetails ClubThread { get; set; }
	public required ClubBar ClubBar { get; set; }

	public async Task<IActionResult> OnGetAsync(long threadId, long clubId)
	{
		var clubThread = await context.ClubThreads
			.Where(ct => ct.Id == threadId)
			.Select(ct => new ThreadDetails
			{
				Id = ct.Id,
				ClubId = ct.ClubId,
				Title = ct.Title,
				IsPinned = ct.IsPinned,
				CreationDate = ct.CreationDate,
				AuthorName = ct.Author.UserName,
				AuthorId = ct.Author.Id,
				AuthorAvatar = ct.Author.Avatar,
				AuthorRole = ct.Author.Roles
					.Where(ur => ur.Order > 0)
					.OrderBy(ur => ur.Order)
					.First(),
				Body = ct.Body,
				CommentsThread = new CommentsThreadDto(ct.CommentsThread.Id, CommentSource.ForumPost, ct.CommentsThread.LockDate),
			})
			.FirstOrDefaultAsync();

		if (clubThread is null) return NotFound();
		ClubThread = clubThread;
		
		var clubBar = await clubRepo.GetClubBar(ClubThread.ClubId);
		if (clubBar is null) return NotFound();
		ClubBar = clubBar;

		return Page();
	}
}