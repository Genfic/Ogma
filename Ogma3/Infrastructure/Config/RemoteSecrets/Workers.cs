using ConfigBoundNET;

namespace Ogma3.Infrastructure.Config.RemoteSecrets;

[ConfigSection("Workers")]
public sealed partial class Workers
{
	public required string AvatarServiceSignatureKey { get; init; }
}