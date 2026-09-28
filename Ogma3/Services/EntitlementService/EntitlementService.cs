using Immediate.Injections.Shared;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Subscriptions;
using Ogma3.Infrastructure.OgmaConfig;
using ZiggyCreatures.Caching.Fusion;

namespace Ogma3.Services.EntitlementService;

[RegisterScoped]
[UsedImplicitly]
public sealed class EntitlementService(
	AppDbContext context,
	IFusionCache cache,
	OgmaConfig config
)
{
	// The cache only drives cosmetic reads, but a short expiry keeps those reads
	// close to the database without needing every mutation site to invalidate.
	private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(15);

	private static string Key(long userId) => $"user:entitlements:{userId}";

	private TimeSpan Grace => TimeSpan.FromDays(config.EntitlementGraceDays);

	public async Task<bool> CheckEntitlement(long userId, Entitlement entitlement)
	{
		var entitlements = await GetEntitlements(userId);

		return entitlements is not null && (entitlements & entitlement) == entitlement;
	}

	public async Task<Entitlement?> GetEntitlements(long userId)
	{
		var grace = Grace;

		return await cache.GetOrSetAsync(Key(userId), async ct => {
			return await context.Subscriptions
				.Where(s => s.UserId == userId)
				.Where(SubscriptionEntitlements.IsActive(grace, DateTimeOffset.UtcNow))
				.Select(s => (Entitlement?)s.Tier!.Entitlements)
				.FirstOrDefaultAsync(ct);
		}, CacheDuration);
	}

	public async Task Clear(long userId)
	{
		await cache.RemoveAsync(Key(userId));
	}
}