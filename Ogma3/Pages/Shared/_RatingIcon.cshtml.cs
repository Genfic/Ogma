using MemoryPack;

namespace Ogma3.Pages.Shared;

[MemoryPackable]
public sealed partial class RatingIcon
{
	public required string Name { get; init; }
	public required string Color { get; init; }
}