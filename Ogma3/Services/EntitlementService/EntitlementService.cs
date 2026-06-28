using Immediate.Injections.Shared;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Subscriptions;
using ZiggyCreatures.Caching.Fusion;

namespace Ogma3.Services.EntitlementService;

[RegisterScoped]
[UsedImplicitly]
public sealed class EntitlementService(
	ApplicationDbContext context,
	IFusionCache cache
)
{
	private static string Key(long userId) => $"user:entitlements:{userId}";

	public async Task<bool> CheckEntitlement(long userId, Entitlement entitlement)
	{
		var entitlements = await GetEntitlements(userId);

		return entitlements is not null && (entitlements & entitlement) == entitlement;
	}

	public async Task<Entitlement?> GetEntitlements(long userId)
	{
		return await cache.GetOrSetAsync(Key(userId), async ct => {
			return await context.Subscriptions
				.Where(s => s.UserId == userId)
				.Select(s => s.Tier == null ? null : (Entitlement?)s.Tier.Entitlements)
				.FirstOrDefaultAsync(ct);
		}, TimeSpan.FromDays(7));
	}

	public async Task Clear(long userId)
	{
		await cache.RemoveAsync(Key(userId));
	}
}