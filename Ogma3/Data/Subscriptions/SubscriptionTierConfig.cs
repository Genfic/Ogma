using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ogma3.Data.Bases;

namespace Ogma3.Data.Subscriptions;

public sealed class SubscriptionTierConfig : BaseConfiguration<SubscriptionTier>
{
	public override void Configure(EntityTypeBuilder<SubscriptionTier> builder)
	{
		base.Configure(builder);

		builder
			.Property(t => t.Name)
			.HasMaxLength(128);
	}
}