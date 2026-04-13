using System.Text.Json;
using System.Text.Json.Serialization;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Users;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Pages.Shared.Bars;
using ZiggyCreatures.Caching.Fusion;

namespace Ogma3.Pages.User.Library;

public sealed class ManageModel
(
	UserRepository userRepo,
	ApplicationDbContext context,
	IFusionCache cache
) : PageModel
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
					.Select(i => new IconDto(i.Id, i.Name))
					.ToListAsync(ct);
				return JsonSerializer.Serialize(icons, IconsJsonContext.Default.ListIconDto);
			},
			options => options.Duration = TimeSpan.FromDays(1));

		return Page();
	}
}

public sealed record IconDto(long Id, string Name);

[UsedImplicitly]
[JsonSerializable(typeof(List<IconDto>))]
public sealed partial class IconsJsonContext : JsonSerializerContext;