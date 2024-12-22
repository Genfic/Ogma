using System.Text.Json.Serialization;

namespace Ogma3.Services.TurnstileService;

public sealed class TurnstileResult
{
	[JsonPropertyName("success")]
	public required bool Success { get; init; }

	[JsonPropertyName("error-codes")]
	public required string[] ErrorCodes { get; init; }

	[JsonPropertyName("challenge_ts")]
	public required DateTimeOffset ChallengeTs { get; init; }

	[JsonPropertyName("hostname")]
	public required string Hostname { get; init; }
}

[JsonSerializable(typeof(TurnstileResult))]
public sealed partial class TurnstileResultContext : JsonSerializerContext;