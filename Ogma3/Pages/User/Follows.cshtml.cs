using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Users;
using Ogma3.Pages.Shared;
using Ogma3.Pages.Shared.Bars;
using Ogma3.Pages.Shared.Cards;

namespace Ogma3.Pages.User;

public sealed class Follows(UserRepository userRepo, ApplicationDbContext context)
	: PageModel
{
	private const int PerPage = 25;

	public required ProfileBar ProfileBar { get; set; }
	public required Pagination Pagination { get; set; }
	public required List<UserCard> Users { get; set; }

	public async Task<ActionResult> OnGetAsync(string name, [FromQuery] int page = 1)
	{
		var profileBar = await userRepo.GetProfileBar(name);
		if (profileBar is null) return NotFound();
		ProfileBar = profileBar;

		Users = await context.FollowedUsers
			.Where(u => u.FollowedUser.NormalizedUserName == name.Normalize().ToUpper())
			.Select(u => u.FollowingUser)
			.Paginate(page, PerPage)
			.Select(UserMappings.ToUserCard)
			.AsNoTracking()
			.ToListAsync();

		var count = await context.Users
			.Where(u => u.NormalizedUserName == name.Normalize().ToUpper())
			.Select(u => u.Following)
			.CountAsync();

		// Prepare pagination
		Pagination = new Pagination
		{
			PerPage = PerPage,
			ItemCount = count,
			CurrentPage = page,
		};

		return Page();
	}
}