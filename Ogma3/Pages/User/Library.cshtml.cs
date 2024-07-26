using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Icons;
using Ogma3.Data.Users;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Pages.Shared.Bars;

namespace Ogma3.Pages.User;

public class LibraryModel(ApplicationDbContext context, UserRepository userRepo) : PageModel
{
	public required bool IsCurrentUser { get; set; }
	public required List<Icon> Icons { get; set; }
	public required ProfileBar ProfileBar { get; set; }

	public async Task<IActionResult> OnGetAsync(string name)
	{
		var profileBar = await userRepo.GetProfileBar(name.ToUpper());
		if (profileBar is null) return NotFound();
		ProfileBar = profileBar;

		IsCurrentUser = name == User.GetUsername();

		Icons = await context.Icons
			.AsNoTracking()
			.ToListAsync();

		return Page();
	}
}