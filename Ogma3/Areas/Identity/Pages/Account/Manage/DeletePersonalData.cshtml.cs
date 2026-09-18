using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Users;
using Index = Routes.Pages.Index;

namespace Ogma3.Areas.Identity.Pages.Account.Manage;

public sealed class DeletePersonalDataModel
(
	UserManager<OgmaUser> userManager,
	SignInManager<OgmaUser> signInManager,
	ILogger<DeletePersonalDataModel> logger,
	AppDbContext context)
	: PageModel
{
	[BindProperty] public required InputModel Input { get; set; }

	public sealed class InputModel
	{
		[Required]
		[DataType(DataType.Password)]
		public required string Password { get; set; }
	}

	public required bool RequirePassword { get; set; }

	public async Task<IActionResult> OnGet()
	{
		var user = await userManager.GetUserAsync(User);
		if (user is null)
		{
			return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
		}

		RequirePassword = await userManager.HasPasswordAsync(user);
		return Page();
	}

	public async Task<IActionResult> OnPostAsync()
	{
		var user = await userManager.GetUserAsync(User);
		if (user is null)
		{
			return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
		}

		RequirePassword = await userManager.HasPasswordAsync(user);
		if (RequirePassword)
		{
			if (!await userManager.CheckPasswordAsync(user, Input.Password))
			{
				ModelState.AddModelError(string.Empty, "Incorrect password.");
				return Page();
			}
		}
		await context.Users
			.Where(u => u.Id == user.Id)
			.ExecuteDeleteAsync();

		await signInManager.SignOutAsync();

		logger.LogInformation("User with ID '{UserId}' deleted their account", user.Id);

		return Index.Get().Redirect(this);
	}
}