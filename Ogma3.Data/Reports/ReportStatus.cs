using NetEscapades.EnumGenerators;
using NpgSqlGenerators;

namespace Ogma3.Data.Reports;

[EnumExtensions]
[PostgresEnum]
public enum ReportStatus
{
	Open,
	InReview,
	Resolved,
	Rejected,
}

public partial class ReportStatusExtensions
{
	extension(ReportStatus status)
	{
		public ReportStatus[] GetNextStatus()
			=> status switch
			{
				ReportStatus.Open => [ReportStatus.InReview],
				ReportStatus.InReview => [ReportStatus.Resolved, ReportStatus.Rejected],
				_ => [],
			};

		public bool CanProgressTo(ReportStatus next)
			=> status.GetNextStatus().Contains(next);
	}
}