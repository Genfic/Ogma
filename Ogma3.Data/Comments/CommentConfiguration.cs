using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ogma3.Data.Bases;
using Ogma3.Data.Constants;

namespace Ogma3.Data.Comments;

public sealed class CommentConfiguration : BaseConfiguration<Comment>
{
	protected override void Config(EntityTypeBuilder<Comment> builder)
	{

		// CONSTRAINTS
		builder
			.Property(c => c.DateTime)
			.IsRequired()
			.HasDefaultValueSql(PgConstants.CurrentTimestamp);

		builder
			.Property(c => c.Body)
			.IsRequired()
			.HasMaxLength(CTConfig.Comment.MaxBodyLength);

		builder
			.Property(c => c.DeletedBy)
			.HasDefaultValue(null);

		builder
			.Property(c => c.DeletedByUserId)
			.HasDefaultValue(null);

		// NAVIGATION
		builder
			.HasOne(c => c.Author)
			.WithMany()
			.OnDelete(DeleteBehavior.SetDefault);
		builder
			.Property(c => c.AuthorId)
			.HasDefaultValue(SystemUserConstants.Deleted.Id);

		builder
			.HasOne(c => c.DeletedByUser)
			.WithMany()
			.HasForeignKey(c => c.DeletedByUserId)
			.OnDelete(DeleteBehavior.SetNull);

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
