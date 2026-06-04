using ConfigBoundNET;

namespace Ogma3.Infrastructure.Config;

[ConfigSection("Patreon")]
public sealed partial class Patreon
{
	public required string AccessToken { get; init; }
	public required string CampaignId { get; init; }
}