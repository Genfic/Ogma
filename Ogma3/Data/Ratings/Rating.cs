#nullable disable

using Ogma3.Data.Bases;

namespace Ogma3.Data.Ratings;

public sealed class Rating : BaseModel
{
	public string Name { get; set; }
	public string Description { get; set; }
	public byte Order { get; set; }
	public string Icon { get; set; }
	public string IconId { get; set; }
	public bool BlacklistedByDefault { get; set; }
}