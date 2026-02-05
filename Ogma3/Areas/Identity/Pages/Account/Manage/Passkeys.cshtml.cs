using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ogma3.Data;

namespace Ogma3.Areas.Identity.Pages.Account.Manage;

public sealed class Passkeys(OgmaUserManager userManager) : PageModel
{
	public sealed record UserPasskey(string? Name, DateTimeOffset CreationDate);

	public List<UserPasskey> UserPasskeys { get; set; } = [];

	public async Task<IActionResult> OnGetAsync()
	{
		var user = await userManager.GetUserAsync(User);
		if (user is null)
		{
			return NotFound();
		}

		var passkeys = await userManager.GetPasskeysAsync(user);
		UserPasskeys = passkeys.Select(p => new UserPasskey(p.Name, p.CreatedAt)).ToList();

		return Page();
	}
}