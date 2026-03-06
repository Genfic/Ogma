using System.Text.Json.Serialization;

namespace Ogma3.Services.PowService;

public sealed class PowChallenge
{
	public required string Token { get; init; }
	public required int Difficulty { get; init; }
	public required string Target { get; init; }
	public required long IssuedAt { get; init; }
}

[JsonSerializable(typeof(PowChallenge))]
public sealed partial class PowChallengeContext : JsonSerializerContext;