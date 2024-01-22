using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Clubs;
using Ogma3.Data.Roles;
using Ogma3.Pages.Shared.Bars;
using Ogma3.Pages.Shared.Cards;

namespace Ogma3.Pages.Club;

public class Members(ClubRepository clubRepo, ApplicationDbContext context) : PageModel
{
	public required ClubBar ClubBar { get;  set; }
	public required List<UserCard> ClubMembers { get; set; }

	public async Task<IActionResult> OnGetAsync(long id)
	{
		var clubBar = await clubRepo.GetClubBar(id);
		if (clubBar is null) return NotFound();
		ClubBar = clubBar;
		
		ClubMembers = await context.ClubMembers
			.Where(cm => cm.ClubId == id)
			.Select(cm => cm.Member)
			.Where(u => u.ClubsBannedFrom.All(c => c.Id != id))
			.Paginate(1, 50)
			.Select(u => new UserCard
			{
				UserName = u.UserName,
				Title = u.Title,
				Avatar = u.Avatar,
				Roles = u.Roles.Select(r => new RoleDto
				{
					Id = r.Id,
					Name = r.Name,
					Order = r.Order,
					Color = r.Color,
					IsStaff = r.IsStaff
				})
			})
			.ToListAsync();

		return Page();
	}
}