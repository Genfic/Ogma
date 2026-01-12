using AutoDbSetGenerators;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ogma3.Data.Bases;
using Ogma3.Infrastructure.Constants;

namespace Ogma3.Data.Comments;

[AutoDbSet]
public sealed class CommentRevision : BaseModel
{
	public DateTimeOffset EditTime { get; init; }
	public required string Body { get; init; }
	public Comment Parent { get; init; } = null!;
	public long ParentId { get; init; }

	public sealed class CommentRevisionConfiguration : BaseConfiguration<CommentRevision>
	{
		public override void Configure(EntityTypeBuilder<CommentRevision> builder)
		{
			base.Configure(builder);

			// CONSTRAINTS
			builder
				.Property(cr => cr.EditTime)
				.IsRequired()
				.HasDefaultValueSql(PgConstants.CurrentTimestamp);

			builder
				.Property(cr => cr.Body)
				.IsRequired()
				.HasMaxLength(CTConfig.Comment.MaxBodyLength);
		}
	}
}