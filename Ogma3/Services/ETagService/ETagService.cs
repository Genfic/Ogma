using StackExchange.Redis;

namespace Ogma3.Services.ETagService;

public sealed class ETagService(IConnectionMultiplexer garnet)
{
	private IDatabase Db => garnet.GetDatabase();
	private static string GetKey<T>(ETagFor etagFor, T id) => $"etag_{etagFor.ToStringFast()}:{id}";

	public void Create<T>(ETagFor etagFor, T id)
	{
		Db.StringSet(GetKey(etagFor, id), Guid.CreateVersion7().ToString());
	}

	public Guid Get<T>(ETagFor etagFor, T id)
	{
		var value = Db.StringGet(GetKey(etagFor, id));
		if (Guid.TryParse(value.ToString(), out var result))
		{
			return result.Version == 7 ? result : Guid.Empty;
		}
		return Guid.Empty;
	}
}