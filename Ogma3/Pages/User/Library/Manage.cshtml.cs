using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Icons;
using Ogma3.Data.Users;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Pages.Shared.Bars;

namespace Ogma3.Pages.User.Library;

public sealed class ManageModel(UserRepository userRepo, ApplicationDbContext context) : PageModel
{
	public required ProfileBar ProfileBar { get; set; }
	public required List<Icon> Icons { get; set; }

	public async Task<IActionResult> OnGetAsync(string username)
	{
		var name = User.GetUsername();
		if (name is null) return Unauthorized();

		var profileBar = await userRepo.GetProfileBar(name.ToUpper());
		if (profileBar is null) return NotFound();
		ProfileBar = profileBar;

		Icons = await context.Icons
			.AsNoTracking()
			.ToListAsync();

		return Page();
	}
}