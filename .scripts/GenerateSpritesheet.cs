#! usr/bin/env dotnet
#:package JetBrains.Annotations@2025.2.4
#:property LangVersion = preview
#:property TargetFramework = net11.0

using System.Collections.Immutable;
using System.Diagnostics;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using JetBrains.Annotations;

var start = Stopwatch.StartNew();

var client = new HttpClient();

var source = args[0];
var target = args[1];

var file = await File.ReadAllTextAsync(source);
var seed = JsonSerializer.Deserialize(file, SpritesheetContext.Default.SeedFile);

if (seed is not { Icons: var icons })
{
	Console.WriteLine("❌ Could not parse seed file.");
	return 1;
}

var grouped = icons
	.Select(i => i.Split(':'))
	.Where(i => i.Length == 2)
	.Select(i => new { Collection = i[0], Name = i[1] })
	.GroupBy(i => i.Collection);

var sprites = (await Task.WhenAll(grouped.Select(async group => {
		var names = string.Join(",", group.Select(x => x.Name));

		var res = await client.GetFromJsonAsync(
			$"https://api.iconify.design/{group.Key}.json?icons={names}",
			SpritesheetContext.Default.IconifyResponse
		);

		return res is { Icons: var i, Height: var h, Width: var w }
			? i.Select(p => new Sprite($"{group.Key}:{p.Key}", p.Value.Body, w, h))
			: null;
	})))
	.SelectMany(x => x ?? [])
	.ToImmutableArray();

var sheet = /*lang=svg*/
	$"""
	 <svg xmlns="http://www.w3.org/2000/svg">
	 	<defs>
	 {string.Join("\n", sprites.Select(s => /*lang=svg*/
	$"""
			<symbol id="{s.Name}" viewBox="0 0 {s.Width} {s.Height}">{s.Body}</symbol>
	"""
	 ))}
	 	</defs>
	 </svg>
	 """;

await File.WriteAllTextAsync(target, sheet);

Console.WriteLine($"Generated icon spritesheet with {sprites.Length} icons in {start.ElapsedMilliseconds:N0}ms");

return 0;

internal sealed record Sprite(string Name, string Body, int Width, int Height);

internal sealed record SeedFile(HashSet<string> Icons);

internal sealed record IconifyResponse(string Prefix, Dictionary<string, Icon> Icons, byte Width, byte Height);

internal sealed record Icon(string Body);

[JsonSerializable(typeof(SeedFile))]
[JsonSerializable(typeof(IconifyResponse))]
[JsonSourceGenerationOptions(PropertyNameCaseInsensitive = true, AllowTrailingCommas = true,
	ReadCommentHandling = JsonCommentHandling.Skip)]
[UsedImplicitly]
internal sealed partial class SpritesheetContext : JsonSerializerContext;