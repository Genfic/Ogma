using AutoDbSetGenerators;
using Ogma3.Data.Bases;

namespace Ogma3.Data.Subscriptions;

[AutoDbSet]
public sealed class SubscriptionTier : BaseModel
{
	public string Name { get; set; } = "";
	public int AmountCents { get; set; }
	public List<Entitlement> Entitlements { get; set; } = []; // TODO: should be a HashSet but it broke. Tracked by #82
}