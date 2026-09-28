using Ogma3.Data.Subscriptions;

namespace Ogma3.Tests.Services;

public sealed class SubscriptionEntitlementsTests
{
	private static readonly DateTimeOffset Now = new(2026, 9, 28, 12, 0, 0, TimeSpan.Zero);
	private static readonly TimeSpan Grace = TimeSpan.FromDays(7);

	private static bool IsActive(Subscription sub, TimeSpan? grace = null)
		=> SubscriptionEntitlements
			.IsActive(grace ?? Grace, Now)
			.Compile()
			// Invoking the compiled predicate mirrors the Where(...) the queries apply
			.Invoke(sub);

	private static Subscription Sub(long? tierId = 1, DateTimeOffset? revokedAt = null)
		=> new() { TierId = tierId, RevokedAt = revokedAt };

	[Test]
	public async Task NeverRevoked_IsActive()
	{
		await Assert.That(IsActive(Sub())).IsTrue();
	}

	[Test]
	public async Task NullTier_IsNotActive()
	{
		await Assert.That(IsActive(Sub(tierId: null))).IsFalse();
	}

	[Test]
	[Arguments(0)]
	[Arguments(60)]
	[Arguments(3600)]
	[Arguments(86400)]
	[Arguments(604799)] // one second inside the 7 day window
	public async Task RevokedWithinGrace_IsActive(int revokedSecondsAgo)
	{
		// The whole point: a transient revoke must not cost the patron their entitlements
		var revoked = Now.AddSeconds(-revokedSecondsAgo);
		await Assert.That(IsActive(Sub(revokedAt: revoked))).IsTrue();
	}

	[Test]
	[Arguments(604801)] // one second past the window
	[Arguments(1209600)] // 14 days
	public async Task RevokedBeyondGrace_IsNotActive(int revokedSecondsAgo)
	{
		var revoked = Now.AddSeconds(-revokedSecondsAgo);
		await Assert.That(IsActive(Sub(revokedAt: revoked))).IsFalse();
	}

	[Test]
	public async Task RevokedAtExactCutoff_IsNotActive()
	{
		// Cutoff itself is not "within" grace; the boundary is exclusive
		await Assert.That(IsActive(Sub(revokedAt: Now - Grace))).IsFalse();
	}

	[Test]
	public async Task LongerGrace_KeepsExpiredSubscriptionActive()
	{
		// A revoked subscription read under a longer grace window is still honoured
		var revoked = Now.AddDays(-10);
		await Assert.That(IsActive(Sub(revokedAt: revoked), TimeSpan.FromDays(30))).IsTrue();
	}

	[Test]
	public async Task NullTier_NotActive_EvenWhenUnrevoked()
	{
		await Assert.That(IsActive(Sub(tierId: null, revokedAt: null))).IsFalse();
	}
}
