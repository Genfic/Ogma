using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ogma3.Data.Bases;
using Ogma3.Infrastructure.Constants;

namespace Ogma3.Data.Comments;

public class CommentConfiguration : BaseConfiguration<Comment>
{
	public override void Configure(EntityTypeBuilder<Comment> builder)
	{
		base.Configure(builder);

		// CONSTRAINTS
		builder
			.Property(c => c.DateTime)
			.IsRequired()
			.HasDefaultValueSql(PgConstants.CurrentTimestamp);

		builder
			.Property(c => c.Body)
			.IsRequired()
			.HasMaxLength(CTConfig.CComment.MaxBodyLength);

		builder
			.Property(c => c.DeletedBy)
			.HasDefaultValue(null);

		builder
			.Property(c => c.DeletedByUserId)
			.HasDefaultValue(null);

		// NAVIGATION
		builder
			.HasOne(c => c.Author)
			.WithMany();

		builder
			.HasOne(c => c.DeletedByUser)
			.WithMany();

		builder
			.HasMany(c => c.Reports)
			.WithOne(r => r.Comment)
			.HasForeignKey(r => r.CommentId)
			.OnDelete(DeleteBehavior.Cascade);

		builder
			.HasMany(c => c.Revisions)
			.WithOne(r => r.Parent)
			.HasForeignKey(r => r.ParentId)
			.OnDelete(DeleteBehavior.Cascade);
	}
}