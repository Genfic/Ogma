using System.Collections.Concurrent;
using JetBrains.Annotations;

namespace Ogma3.Services.IconService;

[RegisterSingleton]
[UsedImplicitly]
public sealed class IconCache(IHttpClientFactory clientFactory, ILogger<IconCache> logger)
{
	private readonly ConcurrentDictionary<string, Icon> _cache = new(StringComparer.OrdinalIgnoreCase);
	private readonly HttpClient _client = clientFactory.CreateClient();

	public async Task<Icon?> GetIcon(string name)
	{
		if (_cache.TryGetValue(name, out var icon))
		{
			return icon;
		}

		logger.LogInformation("Icon cache miss. Fetching icon {IconName}", name);

		if (name.Split(':') is not [var collection, var iconName])
		{
			return null;
		}

		var response = await _client.GetFromJsonAsync(
			$"https://api.iconify.design/{collection}.json?icons={iconName}",
			IconifyJsonContext.Default.IconifyResponse
		);

		logger.LogInformation("Fetched iconify response: {@Response}", response);

		if (response is not { Width: var width, Height: var height, Icons: {} icons } || !icons.TryGetValue(iconName, out var value))
		{
			return null;
		}

		var newIcon = new Icon(width, height, value.Body);

		return _cache.TryAdd(name, newIcon)
			? newIcon
			: null;
	}

	public sealed record Icon(int Width, int Height, string Body);
}