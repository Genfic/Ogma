using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.SecurityTransactions;
using Ogma3.Data.Users;
using Ogma3.Services.Security;
using Index = Routes.Pages.Index;

namespace Ogma3.Areas.Identity.Pages.Account.Manage;

public sealed class DeletePersonalDataModel
(
	UserManager<OgmaUser> userManager,
	SignInManager<OgmaUser> signInManager,
	SecurityAuthorizationService securityAuth,
	ILogger<DeletePersonalDataModel> logger,
	AppDbContext context)
	: PageModel
{
	public async Task<IActionResult> OnGet()
	{
		var user = await userManager.GetUserAsync(User);
		if (user is null)
		{
			return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
		}

		return Page();
	}

	public async Task<IActionResult> OnPostAsync()
	{
		var user = await userManager.GetUserAsync(User);
		if (user is null)
		{
			return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
		}

		// Deleting the account requires fresh account proof (password + TOTP, or a passkey);
		// the old inline password prompt is replaced by the shared reauthentication gate
		if (!await securityAuth.HasVerifiedGrantAsync(user, SecurityOperation.DeleteAccount))
		{
			var txn = await securityAuth.BeginAsync(user, SecurityOperation.DeleteAccount);
			return ReauthRequired.RedirectToReauth(txn, Request.Path.ToString(), this);
		}

		if (!await securityAuth.ConsumeAsync(user, SecurityOperation.DeleteAccount))
		{
			var txn = await securityAuth.BeginAsync(user, SecurityOperation.DeleteAccount);
			return ReauthRequired.RedirectToReauth(txn, Request.Path.ToString(), this);
		}

		await context.Users
			.Where(u => u.Id == user.Id)
			.ExecuteDeleteAsync();

		await signInManager.SignOutAsync();

		logger.LogInformation("User with ID '{UserId}' deleted their account", user.Id);

		return Index.Get().Redirect(this);
	}
}
