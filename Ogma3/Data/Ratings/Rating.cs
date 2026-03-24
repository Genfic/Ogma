using AutoDbSetGenerators;
using Ogma3.Data.Bases;

namespace Ogma3.Data.Ratings;

[AutoDbSet]
public sealed class Rating : BaseModel
{
	public required string Name { get; init; }
	public required string Description { get; init; }
	public byte Order { get; init; } = 0;
	public required string Color { get; init; }
	public bool BlacklistedByDefault { get; init; } = false;
}