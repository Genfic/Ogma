using System.ComponentModel.DataAnnotations;

namespace Ogma3.Services.TurnstileService;

public sealed class TurnstileSettings
{
	public const string Section = "Turnstile";

	[Required]
	public required string Secret { get; init; }
	[Required]
	public required string SiteKey { get; init; }
}