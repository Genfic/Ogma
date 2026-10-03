using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ogma3.Data.Bases;
using Ogma3.Data.Constants;

namespace Ogma3.Data.Reports;

public sealed class ReportConfiguration : BaseConfiguration<Report>
{
	protected override void Config(EntityTypeBuilder<Report> builder)
	{
		builder
			.Property(b => b.ReportDate)
			.IsRequired()
			.HasDefaultValueSql(PgConstants.CurrentTimestamp);

		builder
			.Property(b => b.Reason)
			.HasMaxLength(CTConfig.Report.MaxReasonLength)
			.IsRequired();

		builder.Property(ct => ct.ContentType)
			.HasComputedColumnSql(
				$"""
				 CASE
				 	WHEN "{nameof(Report.ChapterId)}"  IS NOT NULL THEN {(short)ReportableContentType.Chapter}
				 	WHEN "{nameof(Report.BlogpostId)}" IS NOT NULL THEN {(short)ReportableContentType.Blogpost}
				    WHEN "{nameof(Report.StoryId)}"    IS NOT NULL THEN {(short)ReportableContentType.Story}
				 	WHEN "{nameof(Report.UserId)}"     IS NOT NULL THEN {(short)ReportableContentType.User}
				 	WHEN "{nameof(Report.CommentId)}"  IS NOT NULL THEN {(short)ReportableContentType.Comment}
				 	WHEN "{nameof(Report.ClubId)}"     IS NOT NULL THEN {(short)ReportableContentType.Club}
				 END
				 """, stored: true);

		builder.Property(ct => ct.ContentId)
			.HasComputedColumnSql(
				$"""
				 CASE
				 	WHEN "{nameof(Report.ChapterId)}"  IS NOT NULL THEN "{nameof(Report.ChapterId)}"
				 	WHEN "{nameof(Report.BlogpostId)}" IS NOT NULL THEN "{nameof(Report.BlogpostId)}"
				    WHEN "{nameof(Report.StoryId)}"    IS NOT NULL THEN "{nameof(Report.StoryId)}"
				 	WHEN "{nameof(Report.UserId)}"     IS NOT NULL THEN "{nameof(Report.UserId)}"
				 	WHEN "{nameof(Report.CommentId)}"  IS NOT NULL THEN "{nameof(Report.CommentId)}"
				 	WHEN "{nameof(Report.ClubId)}"     IS NOT NULL THEN "{nameof(Report.ClubId)}"
				 END
				 """, stored: true);
	}
}