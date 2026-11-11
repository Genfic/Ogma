using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Ogma3.Services.PasskeyMetadata;

// Shape of the C-MDS blob document served from https://c-mds.fidoalliance.org/:
// a flat object with a "no" serial number and one property per AAGUID
// (each holding friendlyNames plus display icons, which we don't use).
[JsonConverter(typeof(CmdsBlobConverter))]
public sealed class CmdsBlob
{
	/// <summary>Serial number of the blob; also always reported in the response's ETag header.</summary>
	public long? No { get; set; }

	public Dictionary<string, CmdsEntry> Entries { get; } = [];
}

public sealed class CmdsEntry
{
	public Dictionary<string, string>? FriendlyNames { get; set; }
}

public sealed class CmdsBlobConverter : JsonConverter<CmdsBlob>
{
	public override CmdsBlob Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if (reader.TokenType != JsonTokenType.StartObject)
		{
			throw new JsonException("The C-MDS blob is not a JSON object");
		}

		var blob = new CmdsBlob();
		while (reader.Read())
		{
			if (reader.TokenType == JsonTokenType.EndObject)
			{
				return blob;
			}

			var propertyName = reader.GetString() ?? throw new JsonException();
			reader.Read();

			if (propertyName == "no")
			{
				blob.No = reader.TokenType == JsonTokenType.Number
					? reader.GetInt64()
					: reader.GetString() is { } raw
						? long.Parse(raw, CultureInfo.InvariantCulture)
						: throw new JsonException();
			}
			else
			{
				var entry = JsonSerializer.Deserialize(ref reader, PasskeyMetadataContext.Default.CmdsEntry);
				if (entry is not null)
				{
					blob.Entries[propertyName] = entry;
				}
			}
		}

		throw new JsonException("Unterminated C-MDS blob");
	}

	public override void Write(Utf8JsonWriter writer, CmdsBlob value, JsonSerializerOptions options)
		=> throw new NotSupportedException("The C-MDS blob is never serialized");
}

[JsonSourceGenerationOptions(
	PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
	PropertyNameCaseInsensitive = true,
	NumberHandling = JsonNumberHandling.AllowReadingFromString
)]
[JsonSerializable(typeof(CmdsBlob))]
[JsonSerializable(typeof(CmdsEntry))]
[JsonSerializable(typeof(Dictionary<Guid, string>))]
internal sealed partial class PasskeyMetadataContext : JsonSerializerContext;
