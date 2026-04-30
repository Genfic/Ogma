using System.ComponentModel.DataAnnotations;

namespace Ogma3.Infrastructure.Config.RemoteSecrets;

public sealed class Workers
{
	[Required]
	public required string AvatarServiceSignatureKey { get; init; }
}