using AutoDbSetGenerators;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ogma3.Data.Bases;
using Ogma3.Data.Users;
using Ogma3.Infrastructure.Constants;

namespace Ogma3.Data.ModeratorActions;

[AutoDbSet]
public sealed class ModeratorAction : BaseModel
{
	public OgmaUser StaffMember { get; init; } = null!;
	public long StaffMemberId { get; init; }
	public required string Description { get; init; }
	public DateTimeOffset DateTime { get; init; }

	public sealed class ModeratorActionConfiguration : BaseConfiguration<ModeratorAction>
	{
		public override void Configure(EntityTypeBuilder<ModeratorAction> builder)
		{
			base.Configure(builder);

			builder
				.Property(ma => ma.Description)
				.HasMaxLength(5000)
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