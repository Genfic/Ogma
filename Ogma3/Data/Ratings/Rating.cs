using AutoDbSetGenerators;
using Ogma3.Data.Bases;

namespace Ogma3.Data.Ratings;

[AutoDbSet]
public sealed class Rating : BaseModel
{
	public required string Name { get; set; }
	public required string Description { get; set; }
	public required byte Order { get; set; }
	public required string Color { get; set; }
	public required bool BlacklistedByDefault { get; set; }
}