using ConfigBinder.Attributes;
using Immediate.Validations.Shared;

namespace Ogma3.Infrastructure.Config;

[Validate]
[ConfigSection("Workers")]
public sealed partial class Workers : IValidationTarget<Workers>
{
	public required string AvatarServiceSignatureKey { get; init; }
	public required string DiscordBotSignatureKey { get; init; }
}