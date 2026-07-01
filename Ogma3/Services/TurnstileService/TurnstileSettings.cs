using ConfigBinder.Attributes;
using Immediate.Validations.Shared;

namespace Ogma3.Services.TurnstileService;

[Validate]
[ConfigSection("Turnstile")]
public sealed partial class TurnstileSettings : IValidationTarget<TurnstileSettings>
{
	public required string Secret { get; init; }
	public required string SiteKey { get; init; }
}