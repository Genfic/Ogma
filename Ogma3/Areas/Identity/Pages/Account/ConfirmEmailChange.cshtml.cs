﻿#nullable disable

using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Ogma3.Data.Users;

namespace Ogma3.Areas.Identity.Pages.Account;

[AllowAnonymous]
public sealed class ConfirmEmailChangeModel : PageModel
{
	private readonly UserManager<OgmaUser> _userManager;
	private readonly SignInManager<OgmaUser> _signInManager;

	public ConfirmEmailChangeModel(UserManager<OgmaUser> userManager, SignInManager<OgmaUser> signInManager)
	{
		_userManager = userManager;
		_signInManager = signInManager;
	}

	[TempData] public required string StatusMessage { get; set; }

	public async Task<IActionResult> OnGetAsync(string userId, string email, string code)
	{
		var user = await _userManager.FindByIdAsync(userId);
		if (user == null)
		{
			return NotFound($"Unable to load user with ID '{userId}'.");
		}

		code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
		var result = await _userManager.ChangeEmailAsync(user, email, code);
		if (!result.Succeeded)
		{
			StatusMessage = "Error changing email.";
			return Page();
		}

		// In our UI email and user name are one and the same, so when we update the email
		// we need to update the user name.
		var setUserNameResult = await _userManager.SetUserNameAsync(user, email);
		if (!setUserNameResult.Succeeded)
		{
			StatusMessage = "Error changing user name.";
			return Page();
		}

		await _signInManager.RefreshSignInAsync(user);
		StatusMessage = "Thank you for confirming your email change.";
		return Page();
	}
}