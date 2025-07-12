using System.Linq.Expressions;
using Riok.Mapperly.Abstractions;

namespace Ogma3.Data.Ratings;

public sealed class RatingApiDto
{
	public required long Id { get; init; }
	public required string Name { get; init; }
	public required string Description { get; init; }
	public required byte Order { get; init; }
	public required bool BlacklistedByDefault { get; init; }
	public required string Color { get; init; }
}

[Mapper]
public static partial class RatingMapper
{
	public static partial RatingApiDto MapToApiDto(this Rating rating);

	public static readonly Expression<Func<Rating, RatingApiDto>> ToApiDto = r => new RatingApiDto
	{
		Id = r.Id,
		Name = r.Name,
		Description = r.Description,
		Order = r.Order,
		BlacklistedByDefault = r.BlacklistedByDefault,
		Color = r.Color,
	};
}