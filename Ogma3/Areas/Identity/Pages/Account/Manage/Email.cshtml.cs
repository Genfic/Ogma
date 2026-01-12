using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Ogma3.Data.Users;

namespace Ogma3.Areas.Identity.Pages.Account.Manage;

public sealed class EmailModel(UserManager<OgmaUser> userManager, IEmailSender emailSender) : PageModel
{
	public required string Username { get; set; }

	public required string Email { get; set; }

	public required bool IsEmailConfirmed { get; set; }

	[TempData] public required string StatusMessage { get; set; }

	[BindProperty] public required InputModel Input { get; set; }

	public sealed class InputModel
	{
		[Required]
		[EmailAddress]
		[Display(Name = "New email")]
		public required string NewEmail { get; set; }
	}

	private async Task LoadAsync(OgmaUser user)
	{
		var email = await userManager.GetEmailAsync(user);

		if (email is null) return;

		Email = email;

		Input = new InputModel
		{
			NewEmail = email,
		};

		IsEmailConfirmed = await userManager.IsEmailConfirmedAsync(user);
	}

	public async Task<IActionResult> OnGetAsync()
	{
		var user = await userManager.GetUserAsync(User);
		if (user is null)
		{
			return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
		}

		await LoadAsync(user);
		return Page();
	}

	public async Task<IActionResult> OnPostChangeEmailAsync()
	{
		var user = await userManager.GetUserAsync(User);
		if (user is null)
		{
			return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
		}

		if (!ModelState.IsValid)
		{
			await LoadAsync(user);
			return Page();
		}

		var email = await userManager.GetEmailAsync(user);
		if (Input.NewEmail != email)
		{
			var userId = await userManager.GetUserIdAsync(user);
			var code = await userManager.GenerateChangeEmailTokenAsync(user, Input.NewEmail);
			var callbackUrl = Url.Page(
				"/Account/ConfirmEmailChange",
				null,
				new { userId, email = Input.NewEmail, code },
				Request.Scheme);
			await emailSender.SendEmailAsync(
				Input.NewEmail,
				"Confirm your email",
				$"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl!)}'>clicking here</a>.");

			StatusMessage = "Confirmation link to change email sent. Please check your email.";
			return RedirectToPage();
		}

		StatusMessage = "Your email is unchanged.";
		return RedirectToPage();
	}

	public async Task<IActionResult> OnPostSendVerificationEmailAsync()
	{
		var user = await userManager.GetUserAsync(User);
		if (user is null)
		{
			return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
		}

		if (!ModelState.IsValid)
		{
			await LoadAsync(user);
			return Page();
		}

		var userId = await userManager.GetUserIdAsync(user);
		var email = await userManager.GetEmailAsync(user);

		if (email is null) return NotFound();

		var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
		code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

		var callbackUrl = Url.Page(
			"/Account/ConfirmEmail",
			null,
			new { area = "Identity", userId, code },
			Request.Scheme);
		await emailSender.SendEmailAsync(
			email,
			"Confirm your email",
			$"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl!)}'>clicking here</a>.");

		StatusMessage = "Verification email sent. Please check your email.";
		return RedirectToPage();
	}
}