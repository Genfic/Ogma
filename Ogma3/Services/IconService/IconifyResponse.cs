using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace Ogma3.Services.IconService;

public sealed record IconifyResponse
{
	[JsonPropertyName("prefix")]
	public required string Prefix { get; init; }

	[JsonPropertyName("icons")]
	public required Dictionary<string, Icon> Icons { get; init; }

	[JsonPropertyName("width")]
	public required byte Width { get; init; }

	[JsonPropertyName("height")]
	public required byte Height { get; init; }
}

[UsedImplicitly]
public sealed record Icon
{
	[JsonPropertyName("body")]
	public required string Body { get; init; }
}

[UsedImplicitly]
[JsonSerializable(typeof(IconifyResponse))]
public sealed partial class IconifyJsonContext : JsonSerializerContext;