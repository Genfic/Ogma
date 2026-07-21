using Immediate.Injections.Shared;
using StackExchange.Redis;
using StackExchange.Redis.KeyspaceIsolation;

namespace Ogma3.Services.ETagService;

[RegisterScoped]
public sealed class ETagService(IConnectionMultiplexer garnet)
{
	private IDatabase Db => garnet.GetDatabase().WithKeyPrefix("etags:");
	private static string GetKey<T>(ETagFor etagFor, T id, long? userId = null)
		=> $"{etagFor.ToStringFast()}:{id}" + (userId.HasValue ? $":{userId}" : "");

	public void Create<T>(ETagFor etagFor, T id, long? userId)
	{
		Db.StringSet(GetKey(etagFor, id, userId), Guid.CreateVersion7().ToString(), TimeSpan.FromHours(1));
	}

	public Guid Get<T>(ETagFor etagFor, T id, long? userId)
	{
		var value = Db.StringGet(GetKey(etagFor, id, userId));
		if (Guid.TryParse(value.ToString(), out var result))
		{
			return result.Version == 7 ? result : Guid.Empty;
		}
		return Guid.Empty;
	}
}
