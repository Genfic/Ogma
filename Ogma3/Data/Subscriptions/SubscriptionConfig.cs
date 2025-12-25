using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ogma3.Data.Bases;
using Ogma3.Infrastructure.Constants;

namespace Ogma3.Data.Subscriptions;

public sealed class SubscriptionConfig : BaseConfiguration<Subscription>
{
	public override void Configure(EntityTypeBuilder<Subscription> builder)
	{
		base.Configure(builder);

		builder
			.HasOne(s => s.User)
			.WithOne(u => u.Subscription)
			.HasForeignKey<Subscription>(s => s.UserId);

		builder
			.HasOne(s => s.Tier)
			.WithMany()
			.HasForeignKey(s => s.TierId);

		builder
			.Property(s => s.PatreonStatus)
			.HasMaxLength(64);

		builder
			.Property(s => s.CreationDate)
			.HasDefaultValueSql(PgConstants.CurrentTimestamp);

		builder
			.Property(s => s.LastChange)
			.HasDefaultValueSql(PgConstants.CurrentTimestamp);
	}
}