#nullable enable


using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ogma3.Data.Bases;
using Ogma3.Data.Blogposts;
using Ogma3.Data.Chapters;
using Ogma3.Data.Clubs;
using Ogma3.Data.Comments;
using Ogma3.Data.Stories;
using Ogma3.Data.Users;
using Ogma3.Infrastructure.Constants;

namespace Ogma3.Data.Reports;

public class Report : BaseModel
{
	public OgmaUser Reporter { get; set; } = null!;
	public long ReporterId { get; set; }
	public DateTime ReportDate { get; set; }
	public string Reason { get; set; } = null!;

	// Blockable content
	public string ContentType { get; set; } = null!;

	public Comment? Comment { get; set; }
	public long? CommentId { get; set; }

	public OgmaUser? User { get; set; }
	public long? UserId { get; set; }

	public Story? Story { get; set; }
	public long? StoryId { get; set; }

	public Chapter? Chapter { get; set; }
	public long? ChapterId { get; set; }

	public Blogpost? Blogpost { get; set; }
	public long? BlogpostId { get; set; }

	public Club? Club { get; set; }
	public long? ClubId { get; set; }

	public class ReportConfiguration : BaseConfiguration<Report>
	{
		public override void Configure(EntityTypeBuilder<Report> builder)
		{
			base.Configure(builder);
			builder
				.Property(b => b.ReportDate)
				.IsRequired()
				.HasDefaultValueSql(PgConstants.CurrentTimestamp);
			builder.Property(b => b.Reason).IsRequired();
			builder.Property(b => b.ContentType).IsRequired();
		}
	}
}