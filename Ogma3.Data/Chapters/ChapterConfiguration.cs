using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ogma3.Data.Bases;
using Ogma3.Data.CommentsThreads;
using Ogma3.Data.Constants;

namespace Ogma3.Data.Chapters;

public sealed class ChapterConfiguration : BaseConfiguration<Chapter>
{
	private readonly ValueComparer<uint[]> _uintArrayComparer = new(
		(a, b) => (a == null && b == null) || (a != null && b != null && Enumerable.SequenceEqual(a, b)),
		a => a.Aggregate(0, (i, v) => HashCode.Combine(i, v.GetHashCode()))
	);

	public override void Configure(EntityTypeBuilder<Chapter> builder)
	{
		base.Configure(builder);

		builder
			.HasIndex(c => c.Signature)
			.HasMethod(PgConstants.IndexTypes.Gin);

		builder.HasIndex(c => c.PublicationDate);
		builder.HasIndex(c => c.CreationDate);

		// CONSTRAINTS
		builder
			.Property(c => c.PublicationDate)
			.HasDefaultValue(null);

		builder
			.Property(c => c.IsVisible)
			.HasDefaultValue(false);

		builder
			.Property(p => p.CreationDate)
			.IsRequired()
			.HasDefaultValueSql(PgConstants.CurrentTimestamp);

		builder
			.Property(c => c.Title)
			.IsRequired()
			.HasMaxLength(CTConfig.Chapter.MaxTitleLength);

		builder
			.Property(c => c.Slug)
			.IsRequired()
			.HasMaxLength(CTConfig.Chapter.MaxTitleLength);

		builder
			.Property(c => c.Body)
			.IsRequired()
			.HasMaxLength(CTConfig.Chapter.MaxBodyLength);

		builder
			.Property(c => c.StartNotes)
			.IsRequired(false)
			.HasMaxLength(CTConfig.Chapter.MaxNotesLength);

		builder
			.Property(c => c.EndNotes)
			.IsRequired(false)
			.HasMaxLength(CTConfig.Chapter.MaxNotesLength);

		builder
			.Property(c => c.Signature)
			.HasColumnType(PgConstants.Types.IntArray)
			.HasConversion(
				v => Array.ConvertAll(v, i => (int)i),
				v => Array.ConvertAll(v, i => (uint)i),
				_uintArrayComparer
			);

		// NAVIGATION
		builder
			.HasOne(c => c.CommentThread)
			.WithOne(ct => ct.Chapter)
			.HasForeignKey<CommentThread>(ct => ct.ChapterId)
			.OnDelete(DeleteBehavior.Cascade);

		builder
			.HasOne(p => p.ContentBlock)
			.WithOne()
			.HasForeignKey<Chapter>(c => c.ContentBlockId)
			.IsRequired(false);

		builder
			.HasMany(c => c.Reports)
			.WithOne(r => r.Chapter)
			.HasForeignKey(r => r.ChapterId)
			.OnDelete(DeleteBehavior.Cascade);

	}
}