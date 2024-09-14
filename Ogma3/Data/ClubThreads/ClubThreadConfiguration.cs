using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ogma3.Data.Bases;
using Ogma3.Data.CommentsThreads;
using Ogma3.Infrastructure.Constants;

namespace Ogma3.Data.ClubThreads;

public sealed class ClubThreadConfiguration : BaseConfiguration<ClubThread>
{
	public override void Configure(EntityTypeBuilder<ClubThread> builder)
	{
		base.Configure(builder);

		// CONSTRAINTS
		builder
			.Property(ct => ct.Title)
			.IsRequired()
			.HasMaxLength(CTConfig.CClubThread.MaxTitleLength);

		builder
			.Property(ct => ct.Body)
			.IsRequired()
			.HasMaxLength(CTConfig.CClubThread.MaxBodyLength);

		builder
			.Property(ct => ct.CreationDate)
			.IsRequired()
			.HasDefaultValueSql(PgConstants.CurrentTimestamp);

		// NAVIGATION
		builder
			.HasOne(ct => ct.Author)
			.WithMany()
			.HasForeignKey(ct => ct.AuthorId)
			.OnDelete(DeleteBehavior.SetNull);

		builder
			.HasOne(b => b.CommentsThread)
			.WithOne(ct => ct.ClubThread)
			.HasForeignKey<CommentsThread>(ct => ct.ClubThreadId)
			.OnDelete(DeleteBehavior.Cascade);
	}
}