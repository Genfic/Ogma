using System.Linq.Expressions;

namespace Ogma3.Data.Subscriptions;

/// <summary>
///     Single source of truth for whether a subscription currently grants entitlements.
///     The UI and the draft retention job must agree, otherwise a user can be shown one
///     entitlement state while their drafts are deleted on the schedule of another.
/// </summary>
public static class SubscriptionEntitlements
{
	/// <summary>
	///     A subscription is active while it is unrevoked, and for <paramref name="grace" /> after a
	///     reported revocation, so a transient revoke cannot strand the user's drafts.
	/// </summary>
	public static Expression<Func<Subscription, bool>> IsActive(TimeSpan grace, DateTimeOffset now)
	{
		var cutoff = now - grace;
		return s => s.TierId != null && (s.RevokedAt == null || s.RevokedAt > cutoff);
	}
}
