using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Clubs;
using Ogma3.Pages.Shared.Bars;
using Ogma3.Pages.Shared.Cards;

namespace Ogma3.Pages.Club.Folders;

public sealed class IndexModel(ClubRepository clubRepo, ApplicationDbContext context) : PageModel
{
	public required ClubBar ClubBar { get; set; }
	public required ICollection<FolderCard> Folders { get; set; }

	public async Task<IActionResult> OnGetAsync(long id)
	{
		var clubBar = await clubRepo.GetClubBar(id);
		if (clubBar is null) return NotFound();
		ClubBar = clubBar;
		
		Folders = await context.Folders
			.Where(f => f.ClubId == id)
			.Where(f => f.ParentFolderId == null)
			.ProjectToCard()
			.ToListAsync();

		return Page();
	}
}