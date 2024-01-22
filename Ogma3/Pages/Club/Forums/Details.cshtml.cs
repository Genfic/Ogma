using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Clubs;
using Ogma3.Data.Roles;
using Ogma3.Pages.Shared;
using Ogma3.Pages.Shared.Bars;

namespace Ogma3.Pages.Club.Forums;

public class DetailsModel(ClubRepository clubRepo, ApplicationDbContext context) : PageModel
{
	public class ThreadDetails
	{
		public required long Id { get; init; }
		public required long ClubId { get; init; }
		public required string Title { get; init; }
		public required string Body { get; init; }
		public required bool IsPinned { get; init; }
		public required DateTime CreationDate { get; init; }
		public required string AuthorName { get; init; }
		public required long AuthorId { get; init; }
		public required string AuthorAvatar { get; init; }
		public OgmaRole? AuthorRole { get; init; }
		public required CommentsThreadDto CommentsThread { get; init; }
	}

	public required ThreadDetails ClubThread { get; set; }
	public required ClubBar ClubBar { get; set; }

	public async Task<IActionResult> OnGetAsync(long threadId)
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
					.Where(ur => ur.Order.HasValue)
					.OrderBy(ur => ur.Order)
					.First(),
				Body = ct.Body,
				CommentsThread = new CommentsThreadDto
				{
					Id = ct.CommentsThread.Id,
					LockDate = ct.CommentsThread.LockDate,
					Type = nameof(Data.ClubThreads.ClubThread)
				}
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