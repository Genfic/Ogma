using MemoryPack;

namespace Ogma3.Services.PatreonService;

[MemoryPackable]
public sealed partial class PatreonTier
{
	public required string Name { get; init; }
	public required string? Description  { get; init; }
	public required int AmountCents { get; init; }
	public required List<PatreonBenefit> Benefits { get; init; }
	public required string Url { get; init; }
}