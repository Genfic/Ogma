using Riok.Mapperly.Abstractions;

namespace Ogma3.Data.Reports;

public sealed class ReportDto
{
	public long Id { get; init; }
	public string ReporterUserName { get; init; } = null!;
	public long ReporterId { get; init; }
	public DateTimeOffset ReportDate { get; init; }

	public string Reason { get; init; } = null!;

	// Blockable content
	public string ContentType { get; init; } = null!;
	public long? CommentId { get; init; }
	public long? CommentCommentsThreadId { get; init; }
	public string? UserUserName { get; init; }

	public long? UserId { get; init; }
	public long? StoryId { get; init; }
	public long? ChapterId { get; init; }
	public long? BlogpostId { get; init; }
	public string? ClubName { get; init; }
	public long? ClubId { get; init; }
}

[Mapper]
public static partial class ReportMapper
{
	public static partial IQueryable<ReportDto> ProjectToDto(this IQueryable<Report> query);
}