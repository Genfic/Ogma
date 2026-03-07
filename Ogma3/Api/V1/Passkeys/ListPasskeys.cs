using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Ogma3.Data;

namespace Ogma3.Api.V1.Passkeys;

using ReturnType = Results<Ok<List<ListPasskeys.UserPasskey>>, NotFound, InternalServerError>;

[Handler]
[Authorize]
[MapGet("api/passkeys/list")]
public static partial class ListPasskeys
{
	public sealed record Query;

	private static async ValueTask<ReturnType> Handle(
		Query _,
		OgmaUserManager userManager,
		IHttpContextAccessor contextAccessor,
		CancellationToken ct
	)
	{
		if (contextAccessor.HttpContext is not { } httpContext)
		{
			return TypedResults.InternalServerError();
		}

		if (await userManager.GetUserAsync(httpContext.User) is not { } user)
		{
			return TypedResults.NotFound();
		}

		var passkeys = await userManager.GetPasskeysAsync(user);
		var mapped = passkeys.Select(p => new UserPasskey(
			Convert.ToBase64String(p.CredentialId),
			p.Name,
			p.CreatedAt)
		).ToList();

		return TypedResults.Ok(mapped);
	}

	public sealed record UserPasskey(string Id, string? Name, DateTimeOffset CreationDate);
}