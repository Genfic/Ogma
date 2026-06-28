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
	public required IReadOnlyList<PatreonTier> PatreonTiers { get; set; }

	public sealed record TierDto(long Id, string Name, int Price, Entitlement Entitlements);

	public async Task<IActionResult> OnGet(long? edit = null)
	{
		Tiers = await context.SubscriptionTiers
			.Select(t => new TierDto(t.Id, t.Name, t.AmountCents, t.Entitlements))
			.ToListAsync();

		PatreonTiers = await patreonService.GetCampaignTiers();

		if (edit is null)
		{
			return Page();
		}

		var tier = await context.SubscriptionTiers
			.Where(t => t.Id == edit)
			.Select(t => new TierData(t.Id, t.Name, t.AmountCents / 100m, t.Entitlements))
			.FirstOrDefaultAsync();

		if (tier is null)
		{
			return NotFound();
		}

		Tier = tier;

		return Page();
	}

	public sealed record TierData(long? Id, string Name, decimal Price, Entitlement? Entitlements);

	[BindProperty] public TierData Tier { get; set; } = null!;

	public async Task<IActionResult> OnPostAsync()
	{
		if (Tier is { Id: null })
		{
			context.SubscriptionTiers.Add(new SubscriptionTier
			{
				Name = Tier.Name,
				AmountCents = (int)Math.Round(Tier.Price * 100),
				Entitlements = Tier.Entitlements ?? Entitlement.None,
			});
			await context.SaveChangesAsync();
		}
		else
		{
			await context.SubscriptionTiers
				.Where(s => s.Id == Tier.Id)
				.ExecuteUpdateAsync(setters => setters
					.SetProperty(s => s.Name, Tier.Name)
					.SetProperty(s => s.AmountCents, (int)Math.Round(Tier.Price * 100))
					.SetProperty(s => s.Entitlements, Tier.Entitlements ?? Entitlement.None));
		}

		return Routes.Areas.Admin.Pages.Subscriptions.Get().Redirect(this);
	}
}