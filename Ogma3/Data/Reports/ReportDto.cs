using Riok.Mapperly.Abstractions;

namespace Ogma3.Data.Reports;

public sealed class ReportDto
{
	public required long Id { get; init; }
	public required string ReporterUserName { get; init; }
	public required DateTimeOffset ReportDate { get; init; }
	public required string Reason { get; init; }
	public required ReportStatus Status { get; init; }

	public required string ContentType { get; init; }
	public required long ContentId { get; init; }
}

[Mapper]
public static partial class ReportMapper
{
	public static partial IQueryable<ReportDto> ProjectToDto(this IQueryable<Report> query);
}