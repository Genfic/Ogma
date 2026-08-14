using Immediate.Injections.Shared;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using ZiggyCreatures.Caching.Fusion;

namespace Ogma3.Services;

[RegisterScoped]
public sealed class TagNamespaceAliasService(AppDbContext ctx, IFusionCache cache)
{
	private static string Key(string name) => $"ns-alias-cache:{name}";

	private readonly FusionCacheEntryOptions _options = new()
	{
		Duration = TimeSpan.FromDays(1),
	};

	public void InvalidateCache()
	{
		cache.Remove(Key(nameof(GetPrefixes)));
		cache.Remove(Key(nameof(GetAliases)));
	}

	public async Task<Dictionary<string, string>> GetPrefixes(CancellationToken cancellationToken = default)
	{
		var aliases = await cache.GetOrSetAsync(Key(nameof(GetPrefixes)), async ct => {
			var dict = await InnerGetPairs(ct);
			return dict.ToDictionary(k => $"{k}:", v => $"{v}:", StringComparer.InvariantCultureIgnoreCase);
		}, _options, cancellationToken);

		return aliases;
	}

	public async Task<Dictionary<string, string>> GetAliases(CancellationToken cancellationToken = default)
	{
		var aliases = await cache.GetOrSetAsync(Key(nameof(GetPrefixes)),
			async ct => await InnerGetPairs(ct),
			_options,
			token: cancellationToken
		);

		return aliases;
	}

	private async ValueTask<Dictionary<string, string>> InnerGetPairs(CancellationToken ct)
	{
		return await ctx.TagNamespaces
			.Where(ns => ns.Alias != null)
			.ToDictionaryAsync(o => o.Alias == null ? "" : o.Alias, o => o.Slug, StringComparer.InvariantCultureIgnoreCase, ct);
	}
}