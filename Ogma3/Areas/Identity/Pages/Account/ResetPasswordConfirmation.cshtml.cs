using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ogma3.Areas.Identity.Pages.Account;

[AllowAnonymous]
public sealed class ResetPasswordConfirmationModel : PageModel
{
	public void OnGet()
	{
	}
}