#nullable disable

using AutoDbSetGenerators;
using Ogma3.Data.Bases;

namespace Ogma3.Data.Ratings;

[AutoDbSet]
public sealed class Rating : BaseModel
{
	public string Name { get; set; }
	public string Description { get; set; }
	public byte Order { get; set; }
	public string Color { get; set; }
	public bool BlacklistedByDefault { get; set; }
}