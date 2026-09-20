using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ogma3.Infrastructure.ServiceRegistrations;
using Ogma3.Services.ModeratorActionService;
using Ogma3.Services.UserService;

namespace Ogma3.Areas.Admin.Pages.Users;

[Authorize(AuthorizationPolicies.RequireAdminRole)]
public sealed class Create(IUserService userService, IModeratorActionService moderatorActionService) : PageModel
{

	[BindProperty]
	public required InputModel Input { get; set; }

	// Handler needed for SafeRouting generator to work
	public IActionResult OnGet() => Page();

	public async Task<IActionResult> OnPost()
	{
		var createResult = await userService.CreateAsync(Input.Username, Input.Email, Input.Password, true);

		if (createResult is { Succeeded: true, User: {} u })
		{
			moderatorActionService.LogUserCreated(u.Id, u.UserName);
			return Page();
		}

		foreach (var error in createResult.Errors)
		{
			ModelState.AddModelError(string.Empty, error.Description);
		}

		return Page();
	}

	public sealed record InputModel
	(
		string Username,
		[property: DataType(DataType.EmailAddress)] string Email,
		[property: DataType(DataType.Password)] string Password
	);
}