using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ogma3.Data;
using Ogma3.Infrastructure.Constants;

namespace Ogma3.Areas.Admin.Pages;

[Authorize(Roles = RoleNames.Admin)]
public class Settings : PageModel
{
	private readonly OgmaConfig _config;

	public Settings(OgmaConfig config)
	{
		_config = config;
	}

	[BindProperty] public OgmaConfig Config { get; set; }

	public void OnGet()
	{
		Config = _config;
	}

	public async Task<IActionResult> OnPostAsync()
	{
		foreach (var prop in typeof(OgmaConfig).GetProperties().Where(p => p.CanWrite))
		{
			prop.SetValue(_config, prop.GetValue(Config, null), null);
		}

		await _config.PersistAsync();
		return RedirectToPage("./Settings");
	}
}