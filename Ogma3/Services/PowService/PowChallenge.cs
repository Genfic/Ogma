using MemoryPack;

namespace Ogma3.Services.PowService;

[MemoryPackable]
public sealed partial class PowChallenge
{
	public required string Token { get; init; }
	public required int Difficulty { get; init; }
	public required string Target { get; init; }
	public required long IssuedAt { get; init; }
}