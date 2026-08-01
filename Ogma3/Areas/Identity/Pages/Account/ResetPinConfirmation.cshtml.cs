using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;

namespace Ogma3.Areas.Identity.Pages.Account;

public sealed class ResetPinConfirmationModel(AppDbContext context) : PageModel
{
	public async Task<IActionResult> OnGetAsync([FromQuery] string code)
	{
		var hash = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(code)));

		var user = await context.Users
			.Where(u => u.SafetyPinResetTokenHash == hash)
			.Where(u => u.SafetyPinResetTokenExpiry > DateTimeOffset.UtcNow)
			.FirstOrDefaultAsync();

		if (user is null)
		{
			return NotFound();
		}

		user.SafetyPinHash = null;
		user.SafetyPinResetTokenHash = null;
		user.SafetyPinResetTokenExpiry = null;
		user.SafetyPinLockedOutUntil = DateTimeOffset.UtcNow.AddHours(24);

		await context.SaveChangesAsync();
		return Page();
	}
}