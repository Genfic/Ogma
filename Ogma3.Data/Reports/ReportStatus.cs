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