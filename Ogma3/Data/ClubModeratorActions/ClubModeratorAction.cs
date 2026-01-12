using AutoDbSetGenerators;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ogma3.Data.Bases;
using Ogma3.Data.Clubs;
using Ogma3.Data.Users;
using Ogma3.Infrastructure.Constants;

namespace Ogma3.Data.ClubModeratorActions;

[AutoDbSet]
public sealed class ClubModeratorAction : BaseModel
{
	public OgmaUser Moderator { get; set; } = null!;
	public long ModeratorId { get; set; }
	public required string Description { get; set; }
	public DateTimeOffset CreationDate { get; set; }
	public Club Club { get; set; } = null!;
	public long ClubId { get; set; }

	public sealed class ModeratorActionConfiguration : BaseConfiguration<ClubModeratorAction>
	{
		public override void Configure(EntityTypeBuilder<ClubModeratorAction> builder)
		{
			base.Configure(builder);

			builder
				.Property(ma => ma.Description)
				.HasMaxLength(5000)
				.IsRequired();
			builder
				.Property(ma => ma.CreationDate)
				.IsRequired()
				.HasDefaultValueSql(PgConstants.CurrentTimestamp);
			builder
				.HasOne(ma => ma.Moderator)
				.WithMany()
				.HasForeignKey(ma => ma.ModeratorId)
				.OnDelete(DeleteBehavior.SetNull);
			builder
				.HasOne(ma => ma.Club)
				.WithMany()
				.HasForeignKey(ma => ma.ModeratorId)
				.OnDelete(DeleteBehavior.Cascade);
		}
	}
}