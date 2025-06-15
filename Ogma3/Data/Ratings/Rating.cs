#nullable disable

using AutoDbSetGenerators;
using MemoryPack;
using Ogma3.Data.Bases;

namespace Ogma3.Data.Ratings;

[AutoDbSet]
[MemoryPackable] // TODO: It's a database class, probably shouldn't be packable. See `Pages/Shared/Cards/_StoryCard.cs:20`
public sealed partial class Rating : BaseModel
{
	public string Name { get; set; }
	public string Description { get; set; }
	public byte Order { get; set; }
	public string Icon { get; set; }
	public string IconId { get; set; }
	public bool BlacklistedByDefault { get; set; }
}