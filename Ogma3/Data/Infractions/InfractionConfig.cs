using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ogma3.Data.Bases;
using Ogma3.Infrastructure.Constants;

namespace Ogma3.Data.Infractions;

public class InfractionConfig : BaseConfiguration<Infraction>
{
	public override void Configure(EntityTypeBuilder<Infraction> builder)
	{
		base.Configure(builder);

		builder.HasIndex(i => i.UserId);
		builder.HasIndex(i => i.Type);
		builder
			.HasIndex(i => i.RemovedAt)
			.HasFilter($"\"{nameof(Infraction.RemovedAt)}\" IS NOT NULL");

		// Constraints
		builder
			.Property(i => i.IssueDate)
			.IsRequired()
			.HasDefaultValueSql(PgConstants.CurrentTimestamp);

		builder
			.Property(i => i.ActiveUntil)
			.IsRequired();

		builder
			.Property(i => i.RemovedAt)
			.HasDefaultValue(null);

		builder
			.Property(i => i.Reason)
			.HasMaxLength(1000)
			.IsRequired();

		builder
			.Property(i => i.Type)
			.IsRequired();

		// Navigation
		builder
			.HasOne(i => i.User)
			.WithMany(u => u.Infractions)
			.HasForeignKey(i => i.UserId);

		builder
			.HasOne(i => i.IssuedBy)
			.WithMany()
			.HasForeignKey(i => i.IssuedById);

		builder
			.HasOne(i => i.RemovedBy)
			.WithMany()
			.HasForeignKey(i => i.RemovedById);
	}
}