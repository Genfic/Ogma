using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ogma3.Infrastructure.Constants;

namespace Ogma3.Data.Clubs;

public class ClubMemberConfiguration : IEntityTypeConfiguration<ClubMember>
{
	public void Configure(EntityTypeBuilder<ClubMember> builder)
	{
		// CONSTRAINTS
		builder
			.Property(cm => cm.Role)
			.IsRequired()
			.HasDefaultValue(EClubMemberRoles.User);

		builder
			.Property(cm => cm.MemberSince)
			.IsRequired()
			.HasDefaultValueSql(PgConstants.CurrentTimestamp);

		// NAVIGATION
		builder
			.HasKey(cm => new { cm.ClubId, cm.MemberId });

		builder
			.HasOne(cm => cm.Club)
			.WithMany(c => c.ClubMembers)
			.HasForeignKey(cm => cm.ClubId)
			.OnDelete(DeleteBehavior.Cascade);

		builder
			.HasOne(cm => cm.Member)
			.WithMany()
			.HasForeignKey(cm => cm.MemberId)
			.OnDelete(DeleteBehavior.Cascade);
	}
}