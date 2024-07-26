using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Clubs;
using Ogma3.Pages.Shared;
using Ogma3.Pages.Shared.Bars;
using Ogma3.Pages.Shared.Cards;

namespace Ogma3.Pages.Club.Forums;

public class IndexModel(ApplicationDbContext context, ClubRepository clubRepo, OgmaConfig config)
	: PageModel
{
	public required ClubBar ClubBar { get; set; }
	public required IList<ThreadCard> ThreadCards { get; set; }
	public required Pagination Pagination { get; set; }

	public async Task<IActionResult> OnGetAsync(long id, [FromQuery] int page = 1)
	{
		var clubBar = await clubRepo.GetClubBar(id);
		if (clubBar is null) return NotFound();
		ClubBar = clubBar;
		
		var query = context.ClubThreads
			.Where(ct => ct.ClubId == id)
			.Where(ct => ct.DeletedAt == null);

		ThreadCards = await query
			.OrderByDescending(ct => ct.IsPinned)
			.ThenByDescending(ct => ct.CreationDate)
			.Paginate(page, config.ClubThreadsPerPage)
			.Select(ct => new ThreadCard
			{
				Id = ct.Id,
				Title = ct.Title,
				ClubId = ct.ClubId,
				IsPinned = ct.IsPinned,
				CreationDate = ct.CreationDate,
				AuthorName = ct.Author.UserName,
				AuthorAvatar = ct.Author.Avatar,
				CommentsCount = ct.CommentsThread.Comments.Count
			})
			.ToListAsync();

		Pagination = new Pagination
		{
			ItemCount = await query.CountAsync(),
			CurrentPage = page,
			PerPage = config.ClubThreadsPerPage
		};

		return Page();
	}
}