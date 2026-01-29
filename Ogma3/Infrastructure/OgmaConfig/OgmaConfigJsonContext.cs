using System.Text.Json;
using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace Ogma3.Infrastructure.OgmaConfig;

[JsonSerializable(typeof(OgmaConfig))]
[JsonSourceGenerationOptions(WriteIndented = true, ReadCommentHandling = JsonCommentHandling.Skip, AllowTrailingCommas = true)]
[UsedImplicitly]
public sealed partial class OgmaConfigJsonContext : JsonSerializerContext;