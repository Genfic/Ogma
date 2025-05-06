using System.ComponentModel;
using NetEscapades.EnumGenerators;
using NpgSqlGenerators;

namespace Ogma3.Data.Stories;

[PostgresEnum]
[EnumExtensions]
public enum EStoryStatus
{
	Unspecified = 0,
	[Description("In Progress")] InProgress = 1,
	Completed = 2,
	[Description("On Hiatus")] OnHiatus = 3,
	Cancelled = 4,
}

public static partial class EStoryStatusExtensions
{
	public static string GetEmoji(this EStoryStatus status) => status switch
	{
		EStoryStatus.InProgress => "⏳",
		EStoryStatus.Completed => "✅",
		EStoryStatus.OnHiatus => "⏸️",
		EStoryStatus.Cancelled => "❌",
		_ => "❓",
	};
}