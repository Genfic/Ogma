using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ogma3.Infrastructure.Constants;
using Ogma3.Infrastructure.OgmaConfig;

namespace Ogma3.Areas.Admin.Pages;

[Authorize(Roles = RoleNames.Admin)]
public sealed class Settings(OgmaConfig config, OgmaConfigPersistence persistence) : PageModel
{
	[BindProperty] public required OgmaConfig Config { get; set; }

	public void OnGet()
	{
		Config = config;
	}

	public async Task<IActionResult> OnPostAsync()
	{
		OgmaConfigMapper.CopyTo(Config, config);
		await persistence.PersistAsync();
		return Routes.Areas.Admin.Pages.Settings.Get().Redirect(this);
	}
}