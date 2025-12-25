using System.Text.Json.Serialization;

namespace Ogma3.Api.Hooks.Patreon;

[JsonSerializable(typeof(PatreonWebhook))]
public sealed partial class PatreonWebhookContext : JsonSerializerContext;

public sealed record PatreonWebhook(
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("data")] WebhookData Data
);

public sealed record WebhookData(
    [property: JsonPropertyName("data")] DataData Data
);

public sealed record DataData(
    [property: JsonPropertyName("attributes")] Attributes Attributes
);

public sealed record Attributes(
    [property: JsonPropertyName("campaign_lifetime_support_cents")] long CampaignLifetimeSupportCents,
    [property: JsonPropertyName("currently_entitled_amount_cents")] long CurrentlyEntitledAmountCents,
    [property: JsonPropertyName("is_follower")] bool IsFollower,
    [property: JsonPropertyName("is_free_trial")] bool IsFreeTrial,
    [property: JsonPropertyName("is_gifted")] bool IsGifted,
    [property: JsonPropertyName("last_charge_date")] DateTimeOffset LastChargeDate,
    [property: JsonPropertyName("last_charge_status")] string LastChargeStatus,
    [property: JsonPropertyName("lifetime_support_cents")] long LifetimeSupportCents,
    [property: JsonPropertyName("next_charge_date")] DateTimeOffset NextChargeDate,
    [property: JsonPropertyName("patron_status")] string PatronStatus,
    [property: JsonPropertyName("pledge_cadence")] long PledgeCadence,
    [property: JsonPropertyName("pledge_relationship_start")] DateTimeOffset PledgeRelationshipStart,
    [property: JsonPropertyName("will_pay_amount_cents")] long WillPayAmountCents,
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("relationships")] Relationships Relationships,
    [property: JsonPropertyName("type")] string Type
);

public sealed record Relationships(
    [property: JsonPropertyName("currently_entitled_tiers")] CurrentlyEntitledTiers CurrentlyEntitledTiers,
    [property: JsonPropertyName("user")] User User
);

public sealed record CurrentlyEntitledTiers(
    [property: JsonPropertyName("data")] Dat[] Data
);

public sealed record Dat(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("type")] string Type
);

public sealed record User(
    [property: JsonPropertyName("data")] Dat Data
);
