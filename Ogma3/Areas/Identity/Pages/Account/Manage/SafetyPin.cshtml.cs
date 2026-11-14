using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.SecurityTransactions;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Services.Security;
using Sodium;

namespace Ogma3.Areas.Identity.Pages.Account.Manage;

public sealed class SafetyPinModel(AppDbContext context, SecurityAuthorizationService securityAuth) : PageModel
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

		// Creating or changing the safety PIN requires fresh account proof. Changing an
		// existing PIN additionally requires the current PIN (verified below). The 24-hour
		// destructive-action cooldown after PIN recovery is untouched by this flow.
		var user = await context.Users.FirstAsync(u => u.Id == uid);
		var operation = HasPin ? SecurityOperation.ChangeSafetyPin : SecurityOperation.CreateSafetyPin;

		if (HasPin)
		{
			var hash = await context.Users
				.Where(u => u.Id == uid)
				.Select(u => u.SafetyPinHash)
				.FirstOrDefaultAsync();

			if (hash is null || FormData.CurrentPin is null)
			{
				ModelState.AddModelError("", "No such pin");
				return Page();
			}

			if (!PasswordHash.ArgonHashStringVerify(hash, FormData.CurrentPin))
			{
				ModelState.AddModelError("", "Pin doesn't match");
				return Page();
			}
		}

		if (!await securityAuth.HasVerifiedGrantAsync(user, operation))
		{
			var txn = await securityAuth.BeginAsync(user, operation);
			return ReauthRequired.RedirectToReauth(txn, Request.Path.ToString(), this);
		}

		if (!await securityAuth.ConsumeAsync(user, operation))
		{
			var txn = await securityAuth.BeginAsync(user, operation);
			return ReauthRequired.RedirectToReauth(txn, Request.Path.ToString(), this);
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
		// The current PIN was already verified before the grant was consumed
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