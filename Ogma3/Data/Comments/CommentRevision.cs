using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ogma3.Data.Bases;
using Ogma3.Infrastructure.Constants;

namespace Ogma3.Data.Comments;

public class CommentRevision : BaseModel
{
	public DateTime EditTime { get; init; }
	public string Body { get; init; }
	public Comment Parent { get; init; }
	public long ParentId { get; init; }

	public class CommentRevisionConfiguration : BaseConfiguration<CommentRevision>
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
				.HasMaxLength(CTConfig.CComment.MaxBodyLength);
		}
	}
}