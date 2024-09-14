using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Users;
using Ogma3.Pages.Shared.Cards;

namespace Ogma3.Pages;

public sealed class StaffModel(ApplicationDbContext context) : PageModel
{
	public required IEnumerable<UserCard> Staff { get; set; }

	public async Task OnGetAsync()
	{
		Staff = await context.Users
			.Where(u => u.Roles.Any(ur => ur.IsStaff))
			.OrderBy(uc => uc.Roles.OrderBy(r => r.Order).First().Order)
			.ProjectToCard()
			.ToArrayAsync();
	}
}