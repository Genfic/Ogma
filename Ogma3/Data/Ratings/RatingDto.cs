using System.Linq.Expressions;

namespace Ogma3.Data.Ratings;

public sealed class RatingDto
{
	public required long Id { get; init; }
	public required string Name { get; init; }
	public required string Description { get; init; }
	public required string Color { get; init; }
}

public static partial class RatingMapper
{
	public static readonly Expression<Func<Rating, RatingDto>> ToDto = r => new RatingDto
	{
		Id = r.Id,
		Name = r.Name,
		Description = r.Description,
		Color = r.Color,
	};
}