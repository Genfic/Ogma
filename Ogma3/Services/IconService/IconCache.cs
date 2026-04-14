using System.Collections.Concurrent;
using JetBrains.Annotations;

namespace Ogma3.Services.IconService;

[RegisterSingleton]
[UsedImplicitly]
public sealed class IconCache(IHttpClientFactory clientFactory, ILogger<IconCache> logger)
{
	private readonly ConcurrentDictionary<string, Icon> _cache = new(StringComparer.OrdinalIgnoreCase);
	private readonly HttpClient _client = clientFactory.CreateClient();

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

		logger.LogInformation("Icon cache miss. Fetching {Count}/{Total} icons.", missing, names.Count);

		var newIcons = new ConcurrentBag<Icon>();

		await Task.WhenAll(groups.Select(async g => {
			var res = await _client.GetFromJsonAsync(
				$"https://api.iconify.design/{g.Key}.json?icons={string.Join(',', g.Value)}",
				IconifyJsonContext.Default.IconifyResponse
			);

			if (res is { Icons: var i, Height: var h, Width: var w })
			{
				foreach (var ico in i)
				{
					var key = string.Concat(g.Key, ':', ico.Key);
					var icon = new Icon(key, w, h, ico.Value.Body);

					_cache.TryAdd(key, icon);
					newIcons.Add(icon);
				}
			}
		}));

		found.AddRange(newIcons);
		return found.AsReadOnly();
	}


	public record struct Icon(string Name, int Width, int Height, string Body);
}