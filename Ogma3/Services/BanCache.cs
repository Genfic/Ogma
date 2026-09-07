using Immediate.Injections.Shared;
using JetBrains.Annotations;
using StackExchange.Redis;
using StackExchange.Redis.KeyspaceIsolation;

namespace Ogma3.Services;

[RegisterSingleton]
[UsedImplicitly]
public sealed class BanCache(IConnectionMultiplexer redis)
{
	private const string Prefix = "user:bans:";
	private readonly IDatabase _db = redis.GetDatabase().WithKeyPrefix(Prefix);

	public async Task<bool> Ban(long userId, DateTimeOffset expiresAt)
	{
		var expiry = expiresAt.Subtract(DateTimeOffset.UtcNow);
		return await _db.StringSetAsync(userId.ToString(), "1", expiry);
	}

	public async Task<bool> Unban(long userId)
	{
		return await _db.KeyDeleteAsync(userId.ToString());
	}

	public async Task<bool> Check(long userId)
	{
		return await _db.KeyExistsAsync(userId.ToString());
	}

	public async Task<int> Reconcile(IEnumerable<ActiveBan> bans)
	{
		var batch = _db.CreateBatch();
		var tasks = bans
			.Select(ban => new
			{
				ban,
				ttl = ban.ExpiresAt - DateTimeOffset.UtcNow,
			})
			.Where(t => t.ttl > TimeSpan.Zero)
			.Select(t => batch.StringSetAsync(t.ban.UserId.ToString(), "1", t.ttl))
			.ToList();

		batch.Execute();
		var res = await Task.WhenAll(tasks);

		return res.Count(x => x);
	}

	public record struct ActiveBan(long UserId, DateTimeOffset ExpiresAt);
}