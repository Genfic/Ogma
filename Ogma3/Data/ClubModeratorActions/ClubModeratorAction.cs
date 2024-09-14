#nullable disable

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ogma3.Data.Bases;
using Ogma3.Data.Clubs;
using Ogma3.Data.Users;
using Ogma3.Infrastructure.Constants;

namespace Ogma3.Data.ClubModeratorActions;

public sealed class ClubModeratorAction : BaseModel
{
	public OgmaUser Moderator { get; set; }
	public long ModeratorId { get; set; }
	public string Description { get; set; }
	public DateTime CreationDate { get; set; }
	public Club Club { get; set; }
	public long ClubId { get; set; }

	public sealed class ModeratorActionConfiguration : BaseConfiguration<ClubModeratorAction>
	{
		public override void Configure(EntityTypeBuilder<ClubModeratorAction> builder)
		{
			base.Configure(builder);

			builder
				.Property(ma => ma.Description)
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