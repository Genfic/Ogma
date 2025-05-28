using Ogma3.Infrastructure.Extensions;
using Ogma3.Services.UserService;
using StackExchange.Redis;

namespace Ogma3.Services;

public sealed class UserActivityService(IConnectionMultiplexer garnet, IUserService userService)
{
	private const string Prefix = "lastActive:";

	private string? UserName => userService.User?.GetUsername();

	public async Task SetLastActiveAsync()
	{
		if (UserName is null)
		{
			return;
		}
		await garnet.GetDatabase().StringSetAsync($"{Prefix}{UserName}", DateTimeOffset.UtcNow.ToString());
	}

	public async Task<DateTimeOffset?> GetLastActiveAsync()
	{
		if (UserName is null)
		{
			return null;
		}
		var value = await garnet.GetDatabase().StringGetAsync($"{Prefix}{UserName}");

		return DateTimeOffset.TryParse(value, out var result) ? result : null;
	}

}