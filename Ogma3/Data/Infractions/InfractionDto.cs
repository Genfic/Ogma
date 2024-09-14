using Riok.Mapperly.Abstractions;

namespace Ogma3.Data.Infractions;

public sealed class InfractionDto
{
	public required long Id { get; init; }
	public required string UserUserName { get; init; }
	public required long UserId { get; init; }
	public required DateTime IssueDate { get; init; }
	public required DateTime ActiveUntil { get; init; }
	public required DateTime? RemovedAt { get; init; }
	public required string Reason { get; init; }
	public required InfractionType Type { get; init; }
	public required string IssuedByUserName { get; init; }
	public required string? RemovedByUserName { get; init; } 
}


[Mapper]
public static partial class InfractionMapper
{
	public static partial IQueryable<InfractionDto> ProjectToResult(this IQueryable<Infraction> q);
	public static partial InfractionDto MapToResult(this Infraction i);
}