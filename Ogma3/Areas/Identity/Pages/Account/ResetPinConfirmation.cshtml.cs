using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;

namespace Ogma3.Areas.Identity.Pages.Account;

public sealed class ResetPinConfirmationModel(AppDbContext context) : PageModel
{
	[BindProperty] public string? Code { get; set; }
	public bool Success { get; private set; }

	public async Task<IActionResult> OnGetAsync([FromQuery] string? code)
	{
		Code = code;
		if (string.IsNullOrWhiteSpace(Code) || Code.Length > 128)
		{
			return NotFound();
		}

		var hash = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(Code)));

		var validToken = await context.Users
			.Where(u => u.SafetyPinResetTokenHash == hash)
			.Where(u => u.SafetyPinResetTokenExpiry > DateTimeOffset.UtcNow)
			.AnyAsync();

		return validToken ? Page() : NotFound();
	}

	public async Task<IActionResult> OnPostAsync()
	{
		if (string.IsNullOrWhiteSpace(Code) || Code.Length > 128)
		{
			return NotFound();
		}

		var hash = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(Code)));
		var now = DateTimeOffset.UtcNow;
		var affected = await context.Users
			.Where(u => u.SafetyPinResetTokenHash == hash)
			.Where(u => u.SafetyPinResetTokenExpiry > now)
			.ExecuteUpdateAsync(update => update
				.SetProperty(u => u.SafetyPinHash, (string?)null)
				.SetProperty(u => u.SafetyPinResetTokenHash, (string?)null)
				.SetProperty(u => u.SafetyPinResetTokenExpiry, (DateTimeOffset?)null)
				.SetProperty(u => u.SafetyPinLockedOutUntil, now.AddHours(24)),
				HttpContext.RequestAborted);

		if (affected == 0) return NotFound();

		Success = true;
		return Page();
	}
}