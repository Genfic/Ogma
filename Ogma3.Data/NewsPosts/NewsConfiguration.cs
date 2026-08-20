using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ogma3.Data.Bases;
using Ogma3.Data.CommentsThreads;
using Ogma3.Data.Constants;

namespace Ogma3.Data.NewsPosts;

public class NewsConfiguration : BaseConfiguration<News>
{
	public override void Configure(EntityTypeBuilder<News> builder)
	{
		base.Configure(builder);

		builder
			.Property(b => b.Title)
			.IsRequired()
			.UseCollation(PgConstants.CollationNames.CaseInsensitive)
			.HasMaxLength(int.MaxValue);

		builder.HasIndex(b => b.Title);
		builder.HasIndex(b => b.PublicationDate);

		builder
			.Property(b => b.Slug)
			.IsRequired()
			.HasMaxLength(int.MaxValue);

		builder
			.Property(b => b.ExcerptCutoff)
			.IsRequired()
			.HasDefaultValue(200);

		builder
			.Property(b => b.CreationDate)
			.IsRequired()
			.HasDefaultValueSql(PgConstants.CurrentTimestamp);

		builder
			.Property(b => b.PublicationDate)
			.HasDefaultValue(null);

		builder
			.Property(b => b.IsVisible)
			.HasDefaultValue(false);

		builder
			.Property(b => b.Body)
			.IsRequired()
			.HasMaxLength(int.MaxValue);

		builder
			.Property(b => b.AuthorId)
			.HasDefaultValue(SystemUserConstants.Deleted.Id);

		// NAVIGATION
		builder
			.HasOne(b => b.Author)
			.WithMany()
			.HasForeignKey(b => b.AuthorId)
			.OnDelete(DeleteBehavior.SetDefault);

		builder
			.HasOne(b => b.CommentThread)
			.WithOne(ct => ct.News)
			.HasForeignKey<CommentThread>(ct => ct.NewsId)
			.OnDelete(DeleteBehavior.Cascade);
	}
}