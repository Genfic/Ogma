using Immediate.Injections.Shared;
using JetBrains.Annotations;
using Ogma3.Data.Tags;
using StackExchange.Redis;
using StackExchange.Redis.KeyspaceIsolation;

namespace Ogma3.Services.TagCache;

[RegisterSingleton]
[UsedImplicitly]
public sealed class TagCache(IConnectionMultiplexer redis)
{
	private readonly IDatabase _db = redis.GetDatabase().WithKeyPrefix("tag:");

	public async Task AddAsync(TagEntry entry)
	{
		var tasks = GetKeys(entry)
			.Select(key => _db.SetAddAsync(key, entry.Id));
		await Task.WhenAll(tasks);
	}

	public async Task<int> AddManyAsync(IEnumerable<TagEntry> entries)
	{
		var groups = entries
			.SelectMany(e => GetKeys(e)
				.Select(key => (Key: key, Id: (RedisValue)e.Id)))
			.GroupBy(x => x.Key);

		var batch = _db.CreateBatch();
		var ops = new List<Task>();

		foreach (var group in groups)
		{
			var ids = group
				.Select(static x => x.Id)
				.Distinct()
				.ToArray();

			ops.Add(batch.SetAddAsync(group.Key, ids));
		}

		batch.Execute();
		await Task.WhenAll(ops);

		return ops.Count;
	}

	public async Task DeleteAsync(TagEntry entry)
	{
		var tasks = GetKeys(entry)
			.Select(key => _db.SetRemoveAsync(key, entry.Id));
		await Task.WhenAll(tasks);
	}

	public async Task UpdateAsync(TagEntry before, TagEntry after)
	{
		var oldKeys = GetKeys(before).ToHashSet();
		var newKeys = GetKeys(after).ToHashSet();

		var batch = _db.CreateBatch();
		var ops = new List<Task>();

		foreach (var key in oldKeys.Except(newKeys))
		{
			ops.Add(batch.SetRemoveAsync(key, before.Id));
		}

		foreach (var key in newKeys.Except(oldKeys))
		{
			ops.Add(batch.SetAddAsync(key, after.Id));
		}

		batch.Execute();
		await Task.WhenAll(ops);
	}

	public async Task<long[]> GetTagIds(string tagName)
	{
		var members = await _db.SetMembersAsync(tagName);

		return members
			.Select(static member => (long)member)
			.ToArray();
	}

	public async Task<long[]> GetTagIds(string[] tagNames)
	{
		if (tagNames.Length == 0)
		{
			return [];
		}

		var batch = _db.CreateBatch();

		var reads = tagNames
			.Select(name => batch.SetMembersAsync(name))
			.ToArray();

		batch.Execute();

		var groups = await Task.WhenAll(reads);

		return groups
			.SelectMany(static members => members)
			.Select(static member => (long)member)
			.Distinct()
			.ToArray();
	}

	private static IEnumerable<RedisKey> GetKeys(TagEntry tag)
	{
		var name = tag.Name.ToLowerInvariant();
		yield return name;

		if (tag.Namespace is {} ns)
		{
			yield return $"{ns.ToLowerInvariant()}:{name}";
		}
	}

}

public sealed record TagEntry(long Id, string Name, string? Namespace)
{
	public TagEntry(long id, string name, ETagNamespace? ns) : this(id, name, ns?.ToStringFast())
	{}
}