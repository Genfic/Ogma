using System.Security.Claims;
using Ogma3.Data;
using Ogma3.Data.Images;
using Ogma3.Data.Shelves;
using Ogma3.Data.Users;
using Ogma3.Infrastructure.Extensions;
using Utils;

namespace Ogma3.Services.UserService;

[RegisterScoped<IUserService>]
public sealed class UserService(IHttpContextAccessor? accessor, OgmaUserManager userManager, ApplicationDbContext context) : IUserService
{
	public ClaimsPrincipal? User => accessor?.HttpContext?.User;
	public long? UserId => User?.GetNumericId();

	public async Task<UserCreationResult> CreateAsync(string username, string email, string password, bool activated = false)
	{
		var user = new OgmaUser
		{
			UserName = username,
			Email = email,
			EmailConfirmed = activated,
			CommentThread = new(),
			Avatar = new Image
			{
				Url = Gravatar.Generate(email),
			},
		};

		return await CreateAsync(user, password);
	}
	public async Task<UserCreationResult> CreateAsync(OgmaUser user, string password)
	{
		await using var transaction = await context.Database.BeginTransactionAsync();

		try
		{
			var createResult = await userManager.CreateAsync(user, password);

			if (!createResult.Succeeded)
			{
				await transaction.RollbackAsync();
				return UserCreationResult.Failed(createResult.Errors.ToList());
			}

			context.Shelves.AddRange(
				new Shelf
				{
					Name = "Favourites",
					IsQuickAdd = true,
					IsPublic = true,
					IconId = 9,
					Owner = user,
					Color = "#ffff00",
				},
				new Shelf
				{
					Name = "Read Later",
					IsQuickAdd = true,
					IconId = 22,
					Owner = user,
					Color = "#0000ff",
				}
			);

			await context.SaveChangesAsync();
			await transaction.CommitAsync();

			return UserCreationResult.Success(user);
		}
		catch
		{
			await transaction.RollbackAsync();
			throw;
		}
	}

}