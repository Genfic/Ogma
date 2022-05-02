using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Ogma3.Data.Users;

namespace Ogma3.Areas.Identity.Pages.Account;

[AllowAnonymous]
public class LogoutModel : PageModel
{
	private readonly SignInManager<OgmaUser> _signInManager;
	private readonly ILogger<LogoutModel> _logger;

	public LogoutModel(SignInManager<OgmaUser> signInManager, ILogger<LogoutModel> logger)
	{
		_signInManager = signInManager;
		_logger = logger;
	}


	public async Task<IActionResult> OnGetAsync(string returnUrl = null)
	{
		await _signInManager.SignOutAsync();
		_logger.LogInformation("User logged out");

		return returnUrl != null
			? RedirectToPage(returnUrl)
			: RedirectToPage("/Index", new { Area = "" });
	}
}