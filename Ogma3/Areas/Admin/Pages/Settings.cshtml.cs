using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Infrastructure.Constants;
using Ogma3.Infrastructure.OgmaConfig;

namespace Ogma3.Areas.Admin.Pages;

[Authorize(Roles = RoleNames.Admin)]
public sealed class Settings(OgmaConfig config, OgmaConfigPersistence persistence, ApplicationDbContext context) : PageModel
{
	[BindProperty] public required OgmaConfig Config { get; set; }
	public required List<string> DocNames { get; set; } = [];

	public async Task<IActionResult> OnGet()
	{
		Config = config;
		DocNames = await context.Documents
			.Where(d => d.RevisionDate == null)
			.Select(d => d.Slug)
			.ToListAsync();

		return Page();
	}

	public async Task<IActionResult> OnPostAsync()
	{
		OgmaConfigMapper.CopyTo(Config, config);
		await persistence.PersistAsync();
		return Routes.Areas.Admin.Pages.Settings.Get().Redirect(this);
	}
}