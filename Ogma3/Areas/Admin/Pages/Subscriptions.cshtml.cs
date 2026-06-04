using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Subscriptions;
using Ogma3.Services.PatreonService;

namespace Ogma3.Areas.Admin.Pages;

public sealed class Subscriptions(ApplicationDbContext context, PatreonService patreonService) : PageModel
{
	public required List<TierDto> Tiers { get; set; }
	public required IReadOnlyList<PatreonTier> Patiers { get; set; }

	public sealed record TierDto(string Name, int Price, HashSet<Entitlement> Entitlements);

	public async Task<IActionResult> OnGet()
	{
		Tiers = await context.SubscriptionTiers
			.Select(t => new TierDto(t.Name, t.AmountCents, t.Entitlements.ToHashSet()))
			.ToListAsync();

		Patiers = await patreonService.GetCampaignTiers();

		return Page();
	}

	public sealed record TierData(string Name, decimal Price, HashSet<Entitlement>? Entitlements);

	[BindProperty] public required TierData Tier { get; set; }

	public async Task<IActionResult> OnPostAsync()
	{
		context.SubscriptionTiers.Add(new SubscriptionTier
		{
			Name = Tier.Name,
			AmountCents = (int)Math.Round(Tier.Price * 100),
			Entitlements = Tier.Entitlements?.ToList() ?? [],
		});
		await context.SaveChangesAsync();

		return Routes.Areas.Admin.Pages.Subscriptions.Get().Redirect(this);
	}
}