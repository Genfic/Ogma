using System.Security.Claims;
using Immediate.Injections.Shared;
using Ogma3.Data;
using Ogma3.Data.Images;
using Ogma3.Data.Shelves;
using Ogma3.Data.Users;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Services.InviteCodeService;
using Utils;

namespace Ogma3.Services.UserService;

[RegisterScoped<IUserService>]
public sealed class UserService(
	IHttpContextAccessor? accessor,
	OgmaUserManager userManager,
	AppDbContext context,
	InviteCodeService.InviteCodeService inviteCodes) : IUserService
{
	public ClaimsPrincipal? User => accessor?.HttpContext?.User;
	public long? UserId => User?.GetNumericId();

	public async Task<UserCreationResult> CreateAsync(string username, string email, string password, bool activated = false, string? inviteCode = null, CancellationToken cancellationToken = default)
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

		return await CreateAsync(user, password, inviteCode, cancellationToken);
	}

	/// <remarks>
	/// Everything written here — the user, their shelves and, when <paramref name="inviteCode" />
	/// is given, the claim on that code — commits as one unit. A registration therefore cannot
	/// leave a user behind without the invite code that authorised it, and a rejected code cannot
	/// burn itself on a user that is rolled back with it.
	/// </remarks>
	public async Task<UserCreationResult> CreateAsync(OgmaUser user, string password, string? inviteCode = null, CancellationToken cancellationToken = default)
	{
		await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

		try
		{
			var createResult = await userManager.CreateAsync(user, password);

			if (!createResult.Succeeded)
			{
				await transaction.RollbackAsync(cancellationToken);
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

			await context.SaveChangesAsync(cancellationToken);

			var redemption = inviteCode is null
				? null
				: (InviteCodeRedemptionResult?)await inviteCodes.RedeemAsync(inviteCode, user.Id, cancellationToken);

			if (redemption is not null and not InviteCodeRedemptionResult.Redeemed)
			{
				await transaction.RollbackAsync(cancellationToken);
				return UserCreationResult.Failed(redemption.Value);
			}

			await transaction.CommitAsync(cancellationToken);

			return UserCreationResult.Success(user, redemption);
		}
		catch
		{
			await transaction.RollbackAsync(cancellationToken);
			throw;
		}
	}

}