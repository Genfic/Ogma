using Immediate.Injections.Shared;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Services.UserService;
using StackExchange.Redis;
using StackExchange.Redis.KeyspaceIsolation;

namespace Ogma3.Services;

[RegisterScoped]
public sealed class UserActivityService(IConnectionMultiplexer garnet, IUserService userService)
{
	private string? UserName => userService.User?.GetUsername();

	private IDatabase Db => garnet.GetDatabase().WithKeyPrefix("activity:");

	public async Task SetLastActiveAsync()
	{
		if (UserName is null)
		{
			return;
		}
		await Db.StringSetAsync(UserName, DateTimeOffset.UtcNow.ToString());
	}

	public async Task<DateTimeOffset?> GetLastActiveAsync()
	{
		if (UserName is null)
		{
			return null;
		}
		var value = await Db.StringGetAsync(UserName);

		return DateTimeOffset.TryParse(value, out var result) ? result : null;
	}

}
