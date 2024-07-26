using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.ClubModeratorActions;
using Ogma3.Data.Clubs;
using Ogma3.Pages.Shared;
using Ogma3.Pages.Shared.Bars;

namespace Ogma3.Pages.Club;

public class Modlog(ApplicationDbContext context, ClubRepository clubRepo) : PageModel
{
	private const int PerPage = 50;

	public required ICollection<ClubModeratorAction> Actions { get;  set; }
	public required ClubBar ClubBar { get; set; }
	public required Pagination Pagination { get; set; }

	public async Task<ActionResult> OnGetAsync(long id, [FromQuery] int page = 1)
	{
		var clubBar = await clubRepo.GetClubBar(id);
		if (clubBar is null) return NotFound();
		ClubBar = clubBar;

		var query = context.ClubModeratorActions
			.Where(cma => cma.ClubId == id);

		Actions = await query
			.OrderByDescending(ma => ma.CreationDate)
			.Paginate(page, PerPage)
			.ToListAsync();

		Pagination = new Pagination
		{
			PerPage = PerPage,
			CurrentPage = page,
			ItemCount = await query.CountAsync()
		};

		return Page();
	}
}