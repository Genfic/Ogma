using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace Ogma3.Api.Hooks.Patreon;

[JsonSerializable(typeof(PatreonWebhook))]
[JsonSourceGenerationOptions(PropertyNameCaseInsensitive = true)]
[UsedImplicitly]
public sealed partial class PatreonWebhookContext : JsonSerializerContext;

public sealed class PatreonWebhook
{
    [JsonPropertyName("type")]
    public required string Type { get; init; }

    [JsonPropertyName("data")]
    public required WebhookData Data { get; init; }
}

[UsedImplicitly]
public sealed class WebhookData
{
    [JsonPropertyName("attributes")]
    public required Attributes Attributes { get; init; }

    [JsonPropertyName("relationships")]
    public required Relationships Relationships { get; init; }
}

[UsedImplicitly]
public sealed class Attributes
{
    [JsonPropertyName("campaign_lifetime_support_cents")]
    public required long CampaignLifetimeSupportCents { get; init; }

    [JsonPropertyName("currently_entitled_amount_cents")]
    public required long CurrentlyEntitledAmountCents { get; init; }

    [JsonPropertyName("is_follower")]
    public required bool IsFollower { get; init; }

    [JsonPropertyName("is_free_trial")]
    public required bool IsFreeTrial { get; init; }

    [JsonPropertyName("is_gifted")]
    public required bool IsGifted { get; init; }

    [JsonPropertyName("last_charge_date")]
    public required DateTimeOffset? LastChargeDate { get; init; }

    [JsonPropertyName("last_charge_status")]
    public required string? LastChargeStatus { get; init; }

    [JsonPropertyName("lifetime_support_cents")]
    public required long LifetimeSupportCents { get; init; }

    [JsonPropertyName("next_charge_date")]
    public required DateTimeOffset NextChargeDate { get; init; }

    [JsonPropertyName("patron_status")]
    public required string PatronStatus { get; init; }

    [JsonPropertyName("pledge_cadence")]
    public required int PledgeCadence { get; init; }

    [JsonPropertyName("pledge_relationship_start")]
    public required DateTimeOffset PledgeRelationshipStart { get; init; }

    [JsonPropertyName("will_pay_amount_cents")]
    public required long WillPayAmountCents { get; init; }
}

[UsedImplicitly]
public sealed class Relationships
{
    [JsonPropertyName("currently_entitled_tiers")]
    public required CurrentlyEntitledTiers CurrentlyEntitledTiers { get; init; }

    [JsonPropertyName("user")]
    public required User User { get; init; }
}

[UsedImplicitly]
public sealed class CurrentlyEntitledTiers
{
    [JsonPropertyName("data")]
    public required Data[] Data { get; init; }
}

[UsedImplicitly]
public sealed class Data
{
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    [JsonPropertyName("type")]
    public required string Type { get; init; }
}

[UsedImplicitly]
public sealed class User
{
    [JsonPropertyName("data")]
    public required Data Data { get; init; }
}