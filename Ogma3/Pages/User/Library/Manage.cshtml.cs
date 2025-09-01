using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Icons;
using Ogma3.Data.Users;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Pages.Shared.Bars;
using ZiggyCreatures.Caching.Fusion;

namespace Ogma3.Pages.User.Library;

public sealed class ManageModel(UserRepository userRepo, ApplicationDbContext context, IFusionCache cache) : PageModel
{
	public required ProfileBar ProfileBar { get; set; }
	public required string IconsJson { get; set; }

	public async Task<IActionResult> OnGetAsync(string username)
	{
		var name = User.GetUsername();
		if (name is null || !string.Equals(name, username, StringComparison.OrdinalIgnoreCase)) return Unauthorized();

		var profileBar = await userRepo.GetProfileBar(name.ToUpper());
		if (profileBar is null) return NotFound();
		ProfileBar = profileBar;

		IconsJson = await cache.GetOrSetAsync("IconsList", async ct => {
				var icons = await context.Icons
					.AsNoTracking()
					.ToListAsync(ct);
				return JsonSerializer.Serialize(icons, IconsJsonContext.Default.ListIcon);
			},
			options => options.Duration = TimeSpan.FromDays(1));

		return Page();
	}
}

[JsonSerializable(typeof(List<Icon>))]
public sealed partial class IconsJsonContext : JsonSerializerContext;