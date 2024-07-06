using System.ComponentModel;
using NpgSqlGenerators;

namespace Ogma3.Data.Stories;

[PostgresEnum]
public enum EStoryStatus
{
	Unspecified = 0,
	[Description("In Progress")] InProgress = 1,
	Completed = 2,
	OnHiatus = 3,
	Cancelled = 4,
}