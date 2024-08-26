#nullable disable

using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Ogma3.Data.Users;
using Ogma3.Services.TurnstileService;

namespace Ogma3.Areas.Identity.Pages.Account;

[AllowAnonymous]
public class ResetPasswordModel(UserManager<OgmaUser> userManager, ITurnstileService turnstile, ILogger<ResetPasswordModel> logger)
	: PageModel
{
	[BindProperty] public InputModel Input { get; set; }

	[Required(ErrorMessage = "Turnstile response is required")]
	[BindProperty(Name = "cf-turnstile-response")]
	public string TurnstileResponse { get; set; } = null!;

	public class InputModel
	{
		[Required,
		 EmailAddress]
		public string Email { get; set; }

		[Required,
		 StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6),
		 DataType(DataType.Password)]
		public string Password { get; set; }

		[DataType(DataType.Password),
		 Display(Name = "Confirm password"),
		 Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
		public string ConfirmPassword { get; set; }

		public string Code { get; set; }
	}

	public IActionResult OnGet(string code = null)
	{
		if (code is null)
		{
			return BadRequest("A code must be supplied for password reset.");
		}

		Input = new InputModel
		{
			Code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code)),
		};
		return Page();
	}

	public async Task<IActionResult> OnPostAsync()
	{
		if (!ModelState.IsValid)
		{
			return Page();
		}

		// Check Turnstile
		var turnstileResponse =
			await turnstile.Verify(TurnstileResponse, Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty);
		if (!turnstileResponse.Success)
		{
			ModelState.TryAddModelError("Turnstile", "Incorrect Turnstile response");
			logger.LogInformation("Password reset attempt with Turnstile errors: {Errors}", (object)turnstileResponse.ErrorCodes);
			return Page();
		}

		var user = await userManager.FindByEmailAsync(Input.Email);
		if (user is null)
		{
			// Don't reveal that the user does not exist
			return RedirectToPage("./ResetPasswordConfirmation");
		}

		var result = await userManager.ResetPasswordAsync(user, Input.Code, Input.Password);
		if (result.Succeeded)
		{
			return RedirectToPage("./ResetPasswordConfirmation");
		}

		foreach (var error in result.Errors)
		{
			ModelState.AddModelError(string.Empty, error.Description);
		}

		return Page();
	}
}