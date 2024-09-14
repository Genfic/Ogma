namespace Ogma3.Data.Ratings;

public sealed class RatingDto
{
	public required long Id { get; init; }
	public required string Name { get; init; }
	public required string Description { get; init; }
	public required string Icon { get; init; }
}

public static partial class RatingMapper
{
	public static partial IQueryable<RatingDto> ProjectToDto(this IQueryable<Rating> source);
	public static partial RatingDto ToDto(this Rating source);
}