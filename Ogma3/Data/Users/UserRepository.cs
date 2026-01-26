using Microsoft.EntityFrameworkCore;
using Ogma3.Pages.Shared.Bars;
using Ogma3.Services.UserService;

namespace Ogma3.Data.Users;

public sealed class UserRepository(ApplicationDbContext context, IUserService userService)
{
	private readonly long? _uid = userService.UserId;

	public async Task<ProfileBar?> GetProfileBar(string name)
	{
		return await context.Users
			.TagWith($"{nameof(UserRepository)}.{nameof(GetProfileBar)} -> {name}")
			.Where(u => u.NormalizedUserName == name)
			.Select(UserMappings.ToProfileBar(_uid))
			.FirstOrDefaultAsync();
	}

	public async Task<ProfileBar?> GetProfileBar(long id)
	{
		return await context.Users
			.TagWith($"{nameof(UserRepository)}.{nameof(GetProfileBar)} -> {id}")
			.Where(u => u.Id == id)
			.Select(UserMappings.ToProfileBar(_uid))
			.FirstOrDefaultAsync();
	}
}