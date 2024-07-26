using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Clubs;
using Ogma3.Data.Folders;
using Ogma3.Pages.Shared.Bars;
using Ogma3.Pages.Shared.Cards;

namespace Ogma3.Pages.Club.Folders;

public class IndexModel(ClubRepository clubRepo, ApplicationDbContext context) : PageModel
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
			.Select(f => new FolderCard
			{
				Id = f.Id,
				Name = f.Name,
				Slug = f.Slug,
				Description = f.Description,
				ClubId = f.ClubId,
				StoriesCount = f.StoriesCount,
				ChildFolders = f.ChildFolders.Select(cf => new FolderMinimalDto
				{
					Id = cf.Id,
					Name = cf.Name,
					Slug = cf.Slug
				})
			})
			.ToListAsync();

		return Page();
	}
}