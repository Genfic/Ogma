using AutoDbSetGenerators;
using Ogma3.Data.Bases;
using Ogma3.Data.Users;

namespace Ogma3.Data.Subscriptions;

[AutoDbSet]
public sealed class Subscription : BaseModel
{
	public OgmaUser User { get; set; } = null!;
	public long UserId { get; set; }

	public SubscriptionTier? Tier { get; set; }
	public long? TierId { get; set; }

	public List<string> PatreonTierIds { get; set; } = [];
	public string PatreonStatus { get; set; } = "";

	public DateTimeOffset CreationDate { get; set; }
	public DateTimeOffset LastChange { get; set; }
}