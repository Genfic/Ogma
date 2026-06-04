using MemoryPack;

namespace Ogma3.Services.PatreonService;

[MemoryPackable]
public sealed partial class PatreonBenefit
{
	public required string Name { get; init; }
	public required string? Description { get; init; }
}