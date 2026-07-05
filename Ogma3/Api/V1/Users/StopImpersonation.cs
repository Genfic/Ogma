using System.Security.Claims;
using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Ogma3.Data;
using Ogma3.Data.Users;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.Users;

using ReturnType = Results<RedirectHttpResult, BadRequest<string>, NotFound>;

[Handler]
[Authorize]
[MapGroup<ApiGroup>]
[MapGet("users/stop-impersonation")]
public sealed partial class StopImpersonation
(
	OgmaUserManager userManager,
	SignInManager<OgmaUser> signInManager,
	IUserService userService
)
{
	[UsedImplicitly]
	public sealed record Query;

	private async ValueTask<ReturnType> Handle(Query _, CancellationToken ct)
	{
		ct.ThrowIfCancellationRequested();

		var originalId = userService.User?.FindFirstValue(ClaimTypes.ImpersonatingUserId);
		if (!long.TryParse(originalId, out var id))
		{
			return TypedResults.BadRequest("You're not impersonating anyone");
		}

		var originalUser = await userManager.FindByIdAsync(id.ToString());
		if (originalUser is null)
		{
			return TypedResults.NotFound();
		}

		await signInManager.SignOutAsync();
		await signInManager.SignInAsync(originalUser, isPersistent: false);

		return TypedResults.Redirect("/");
	}
}