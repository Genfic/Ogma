using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ogma3.Data.Bases;
using Ogma3.Infrastructure.Constants;

namespace Ogma3.Data.Clubs;

public class ClubConfiguration : BaseConfiguration<Club>
{
	public override void Configure(EntityTypeBuilder<Club> builder)
	{
		base.Configure(builder);

		// CONSTRAINTS
		builder
			.Property(c => c.Name)
			.IsRequired()
			.HasMaxLength(CTConfig.CClub.MaxNameLength);

		builder
			.Property(c => c.Slug)
			.IsRequired()
			.HasMaxLength(CTConfig.CClub.MaxNameLength);

		builder
			.Property(c => c.Hook)
			.IsRequired()
			.HasMaxLength(CTConfig.CClub.MaxHookLength);

		builder
			.Property(c => c.Description)
			.HasMaxLength(CTConfig.CClub.MaxDescriptionLength);

		builder
			.Property(c => c.CreationDate)
			.IsRequired()
			.HasDefaultValueSql(PgConstants.CurrentTimestamp);


		// NAVIGATION
		builder
			.HasMany(c => c.Threads)
			.WithOne(ct => ct.Club)
			.HasForeignKey(ct => ct.ClubId)
			.OnDelete(DeleteBehavior.Cascade);

		builder
			.HasMany(c => c.Folders)
			.WithOne(f => f.Club)
			.HasForeignKey(f => f.ClubId)
			.OnDelete(DeleteBehavior.Cascade);

		builder
			.HasMany(c => c.Reports)
			.WithOne(r => r.Club)
			.HasForeignKey(r => r.ClubId)
			.OnDelete(DeleteBehavior.Cascade);

		builder
			.HasMany(c => c.BannedUsers)
			.WithMany(u => u.ClubsBannedFrom)
			.UsingEntity<ClubBan>(ent => ent
					.HasOne(cb => cb.User)
					.WithMany()
					.HasForeignKey(cb => cb.UserId)
					.OnDelete(DeleteBehavior.Cascade),
				ent => ent
					.HasOne(cb => cb.Club)
					.WithMany()
					.HasForeignKey(cb => cb.ClubId)
					.OnDelete(DeleteBehavior.Cascade)
			);
	}
}