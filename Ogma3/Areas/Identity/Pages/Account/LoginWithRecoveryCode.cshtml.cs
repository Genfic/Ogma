﻿using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ogma3.Data.Users;

namespace Ogma3.Areas.Identity.Pages.Account;

[AllowAnonymous]
public sealed class LoginWithRecoveryCodeModel(SignInManager<OgmaUser> signInManager, ILogger<LoginWithRecoveryCodeModel> logger)
	: PageModel
{
	[BindProperty] public required InputModel Input { get; set; }

	public string? ReturnUrl { get; set; }

	public sealed class InputModel
	{
		[BindProperty]
		[Required]
		[DataType(DataType.Text)]
		[Display(Name = "Recovery Code")]
		public required string RecoveryCode { get; set; }
	}

	public async Task<IActionResult> OnGetAsync(string? returnUrl = null)
	{
		// Ensure the user has gone through the username & password screen first
		var user = await signInManager.GetTwoFactorAuthenticationUserAsync();
		if (user is null)
		{
			throw new InvalidOperationException("Unable to load two-factor authentication user.");
		}

		ReturnUrl = returnUrl;

		return Page();
	}

	public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
	{
		if (!ModelState.IsValid)
		{
			return Page();
		}

		var user = await signInManager.GetTwoFactorAuthenticationUserAsync();
		if (user == null)
		{
			throw new InvalidOperationException("Unable to load two-factor authentication user.");
		}

		var recoveryCode = Input.RecoveryCode.Replace(" ", string.Empty);

		var result = await signInManager.TwoFactorRecoveryCodeSignInAsync(recoveryCode);

		if (result.Succeeded)
		{
			logger.LogInformation("User with ID '{UserId}' logged in with a recovery code", user.Id);
			return LocalRedirect(returnUrl ?? Url.Content("~/"));
		}

		if (result.IsLockedOut)
		{
			logger.LogWarning("User with ID '{UserId}' account locked out", user.Id);
			return RedirectToPage("./Lockout");
		}

		logger.LogWarning("Invalid recovery code entered for user with ID '{UserId}' ", user.Id);
		ModelState.AddModelError(string.Empty, "Invalid recovery code entered.");
		return Page();
	}
}