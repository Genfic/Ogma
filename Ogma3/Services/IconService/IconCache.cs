using System.Collections.Concurrent;
using Immediate.Injections.Shared;
using JetBrains.Annotations;

namespace Ogma3.Services.IconService;

[RegisterSingleton]
[UsedImplicitly]
public sealed class IconCache(IHttpClientFactory clientFactory, ILogger<IconCache> logger)
{
	private readonly ConcurrentDictionary<string, Icon> _cache = new(StringComparer.OrdinalIgnoreCase);

	public async ValueTask<IReadOnlyList<Icon>> GetIcons(HashSet<string> names)
	{
		var found = new List<Icon>(names.Count);

		var groups = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);

		var missing = 0;
		foreach (var name in names)
		{
			if (_cache.TryGetValue(name, out var icon))
			{
				found.Add(icon);
				continue;
			}

			var span = name.AsSpan();
			var colon = span.IndexOf(':');
			if (colon < 0)
			{
				continue;
			}

			var collection = span[..colon].ToString();
			var iconName = span[(colon + 1)..].ToString();

			if (!groups.TryGetValue(collection, out var set))
			{
				groups[collection] = set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			}

			missing++;
			set.Add(iconName);
		}

		if (groups.Count <= 0)
		{
			return found;
		}

		var loggedNames = groups.SelectMany(g => g.Value.Select(v => $"{g.Key}:{v}"));
		logger.LogInformation("Icon cache miss. Fetching {Count}/{Total} icons: {@Names}.", missing, names.Count, loggedNames);

		var newIcons = new ConcurrentBag<Icon>();
		var client = clientFactory.CreateClient();

		await Task.WhenAll(groups.Select(async g => {
			var res = await client.GetFromJsonAsync(
				$"https://api.iconify.design/{g.Key}.json?icons={string.Join(',', g.Value)}",
				IconifyJsonContext.Default.IconifyResponse
			);

			if (res is null)
			{
				return;
			}

			var aliasMap = res.Aliases.ToDictionary(a => a.Value.Parent, a => a.Key);

			foreach (var ico in res.Icons)
			{
				var name = aliasMap.TryGetValue(ico.Key, out var alias) ? alias : ico.Key;

				var key = $"{g.Key}:{name}";
				var icon = new Icon(key, res.Width, res.Height, ico.Value.Body);

				_cache.TryAdd(key, icon);
				newIcons.Add(icon);
			}

		}));

		found.AddRange(newIcons);
		return found.AsReadOnly();
	}


	public record struct Icon(string Name, int Width, int Height, string Body);
}