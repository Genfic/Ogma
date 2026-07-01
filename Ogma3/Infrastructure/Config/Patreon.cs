using ConfigBinder.Attributes;
using Immediate.Validations.Shared;

namespace Ogma3.Infrastructure.Config;

[Validate]
[ConfigSection("Patreon")]
public sealed partial class Patreon : IValidationTarget<Patreon>
{
	public required string AccessToken { get; init; }
	public required string CampaignId { get; init; }
	public required string WebhookSecret { get; init; }
}