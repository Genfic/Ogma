#nullable disable

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ogma3.Data.Bases;
using Ogma3.Data.Users;
using Ogma3.Infrastructure.Constants;

namespace Ogma3.Data.ModeratorActions;

public sealed class ModeratorAction : BaseModel
{
	public OgmaUser StaffMember { get; init; }
	public long StaffMemberId { get; init; }
	public string Description { get; init; }
	public DateTimeOffset DateTime { get; init; }

	public sealed class ModeratorActionConfiguration : BaseConfiguration<ModeratorAction>
	{
		public override void Configure(EntityTypeBuilder<ModeratorAction> builder)
		{
			base.Configure(builder);

			builder
				.Property(ma => ma.Description)
				.IsRequired();
			builder
				.Property(ma => ma.DateTime)
				.IsRequired()
				.HasDefaultValueSql(PgConstants.CurrentTimestamp);
			builder
				.HasOne(ma => ma.StaffMember)
				.WithMany()
				.HasForeignKey(ma => ma.StaffMemberId)
				.OnDelete(DeleteBehavior.SetNull);
		}
	}
}