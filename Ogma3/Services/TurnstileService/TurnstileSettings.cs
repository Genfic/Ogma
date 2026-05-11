using ConfigBoundNET;

namespace Ogma3.Services.TurnstileService;

[ConfigSection("Turnstile")]
public sealed partial class TurnstileSettings
{
	public required string Secret { get; init; }
	public required string SiteKey { get; init; }
}