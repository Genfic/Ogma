using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Services.Mailer;
using Sodium;

namespace Ogma3.Areas.Identity.Pages.Account.Manage;

public sealed class SafetyPinModel(ApplicationDbContext context, IMailer mailer, ILogger<SafetyPinModel> logger) : PageModel
{
	[BindProperty]
	public required Data FormData { get; set; }

	public required bool HasPin { get; set; }
	public TimeSpan? TimeSinceLockout { get; set; }
	public required bool Success { get; set; }

	public async Task<IActionResult> OnGet()
	{
		if (User.GetNumericId() is not {} uid)
		{
			return Page();
		}

		var data = await context.Users
			.Where(u => u.Id == uid)
			.Select(u => new
			{
				HasPin = u.SafetyPinHash != null,
				Lockout = u.SafetyPinLockedOutUntil,
			})
			.FirstOrDefaultAsync();

		if (data is null)
		{
			return NotFound();
		}

		HasPin = data.HasPin;
		TimeSinceLockout = data.Lockout is null ? null : data.Lockout - DateTimeOffset.UtcNow;

		return Page();
	}

	public async Task<IActionResult> OnPostAsync()
	{
		if (User.GetNumericId() is not {} uid)
		{
			return Page();
		}

		HasPin = await context.Users
			.Where(u => u.Id == uid)
			.Select(u => u.SafetyPinHash != null)
			.FirstOrDefaultAsync();

		if (!ModelState.IsValid)
		{
			return Page();
		}

		Success = HasPin
			? await UpdatePin(uid)
			: await CreatePin(uid);

		return Page();
	}

	private async Task<bool> CreatePin(long uid)
	{
		var hash = PasswordHash.ArgonHashString(FormData.Pin);

		var rows = await context.Users
			.Where(u => u.Id == uid)
			.ExecuteUpdateAsync(s => s.SetProperty(u => u.SafetyPinHash, hash));

		return rows > 0;
	}

	private async Task<bool> UpdatePin(long uid)
	{
		var hash = await context.Users
			.Where(u => u.Id == uid)
			.Select(u => u.SafetyPinHash)
			.FirstOrDefaultAsync();

		if (hash is null || FormData.CurrentPin is null)
		{
			ModelState.AddModelError("", "No such pin");
			return false;
		}

		var check = PasswordHash.ArgonHashStringVerify(hash, FormData.CurrentPin);

		if (!check)
		{
			ModelState.AddModelError("", "Pin doesn't match");
			return false;
		}

		var newHash = PasswordHash.ArgonHashString(FormData.Pin);

		var rows = await context.Users
			.Where(u => u.Id == uid)
			.ExecuteUpdateAsync(s => s.SetProperty(u => u.SafetyPinHash, newHash));

		return rows > 0;
	}

	public sealed class Data
	{
		[Required]
		[StringLength(6)]
		[Display(Name = "New PIN")]
		public required string Pin { get; set; }

		[Required]
		[StringLength(6)]
		[Compare(nameof(Pin))]
		[Display(Name = "Confirm PIN")]
		public required string RepeatPin { get; set; }

		[StringLength(6)]
		[Display(Name = "Current PIN")]
		public string? CurrentPin { get; set; }
	}
}