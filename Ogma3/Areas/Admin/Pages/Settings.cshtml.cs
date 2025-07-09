using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ogma3.Data;
using Ogma3.Infrastructure.Constants;

namespace Ogma3.Areas.Admin.Pages;

[Authorize(Roles = RoleNames.Admin)]
public sealed class Settings(OgmaConfig config) : PageModel
{
	[BindProperty] public required OgmaConfig Config { get; set; }

	public void OnGet()
	{
		Config = config;
	}

	public async Task<IActionResult> OnPostAsync()
	{
		foreach (var prop in typeof(OgmaConfig).GetProperties().Where(p => p.CanWrite))
		{
			prop.SetValue(config, prop.GetValue(Config, null), null);
		}

		await config.PersistAsync();
		return Routes.Pages.Settings.Get().Redirect(this);
	}
}