using Riok.Mapperly.Abstractions;

namespace Ogma3.Data.Ratings;

public sealed class RatingApiDto
{
	public required long Id { get; init; }
	public required string Name { get; init; }
	public required string Description { get; init; }
	public required byte Order { get; init; }
	public required string Icon { get; init; }
	public required bool BlacklistedByDefault { get; init; }
}

[Mapper]
public static partial class RatingMapper
{
	public static partial RatingApiDto MapToApiDto(this Rating rating);
	public static partial IQueryable<RatingApiDto> ProjectToApiDto(this IQueryable<Rating> query);
}