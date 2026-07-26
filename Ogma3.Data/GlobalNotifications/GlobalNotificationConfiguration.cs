using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ogma3.Data.Bases;
using Ogma3.Data.Constants;

namespace Ogma3.Data.GlobalNotifications;

public class GlobalNotificationConfiguration : BaseConfiguration<GlobalNotification>
{
	public override void Configure(EntityTypeBuilder<GlobalNotification> builder)
	{
		base.Configure(builder);

		builder
			.Property(n => n.Message)
			.HasMaxLength(512);

		builder
			.Property(n => n.CreatedAt)
			.HasDefaultValueSql(PgConstants.CurrentTimestamp);

		builder
			.Property(n => n.Color)
			.HasMaxLength(8);

		builder
			.HasOne(n => n.CreatedBy)
			.WithMany()
			.HasForeignKey(n => n.CreatedById);
	}
}