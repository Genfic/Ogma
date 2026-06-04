using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ogma3.Services.PatreonService;

namespace Ogma3.Areas.Identity.Pages.Account.Manage;

public sealed class Subscription(PatreonService patreonService) : PageModel
{
	public required IReadOnlyList<PatreonTier> Tiers { get; set; }

	public async Task<IActionResult> OnGet()
	{
		Tiers = await patreonService.GetCampaignTiers(true);

		return Page();
	}
}