using System.Text.Json.Serialization;

namespace Ogma3.Services.PatreonService;

public sealed class CampaignData
{
	public required Data Data { get; init; }
	public required List<Inclusion> Included { get; init; }
}

public sealed class Data
{
	public required string Id { get; init; }
}

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type", IgnoreUnrecognizedTypeDiscriminators = true)]
[JsonDerivedType(typeof(BenefitInclusion), "benefit")]
[JsonDerivedType(typeof(TierInclusion), "tier")]
public abstract class Inclusion
{
	public required string Id { get; init; }
}

public sealed class BenefitInclusion : Inclusion
{
	public required BenefitAttributes Attributes { get; init; }
}

public sealed class TierInclusion : Inclusion
{
	public required TierAttributes Attributes { get; init; }

	public Relationships? Relationships { get; init; }
}

public sealed class BenefitAttributes
{
	public required string Title { get; init; }
	public string? Description { get; init; }
}

public sealed class TierAttributes
{
	public required int AmountCents { get; init; }
	public required string Title { get; init; }
	public string? Description { get; init; }
	public required string Url { get; init; }
}

public sealed class Relationships
{
	public required Benefits Benefits { get; init; }
}

public sealed class Benefits
{
	public required List<Data> Data { get; init; }
}

[JsonSerializable(typeof(CampaignData))]
[JsonSerializable(typeof(BenefitInclusion))]
[JsonSerializable(typeof(TierInclusion))]
[JsonSourceGenerationOptions(
	PropertyNamingPolicy = JsonKnownNamingPolicy.SnakeCaseLower,
	AllowOutOfOrderMetadataProperties = true
)]
public sealed partial class PatreonDataContext : JsonSerializerContext;