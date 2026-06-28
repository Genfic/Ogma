using AutoDbSetGenerators;
using Ogma3.Data.Bases;

namespace Ogma3.Data.Subscriptions;

[AutoDbSet]
public sealed class SubscriptionTier : BaseModel
{
	public string Name { get; set; } = "";
	public int AmountCents { get; set; }
	public Entitlement Entitlements { get; set; }
}