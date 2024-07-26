using System.Text.Json.Serialization;

namespace Ogma3.Services.TurnstileService;

public record TurnstileResult(
	[property: JsonPropertyName("success")] bool Success,
	[property: JsonPropertyName("error-codes")] string[] ErrorCodes,
	[property: JsonPropertyName("challenge_ts")] DateTimeOffset ChallengeTs,
	[property: JsonPropertyName("hostname")] string Hostname);

[JsonSerializable(typeof(TurnstileResult))]
public partial class TurnstileResultContext : JsonSerializerContext;