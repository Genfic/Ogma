using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Ogma3.Data;
using Ogma3.Services.PasskeyMetadata;

namespace Ogma3.Api.V1.Passkeys;

using ReturnType = Results<Ok<List<ListPasskeys.UserPasskey>>, NotFound, InternalServerError>;

[Handler]
[MapGroup<ApiGroup>]
[Authorize]
[MapGet("passkeys/list")]
public sealed partial class ListPasskeys(OgmaUserManager userManager, IHttpContextAccessor contextAccessor, PasskeyMetadataService passkeyMetadata)
{

	private async ValueTask<ReturnType> Handle(
		Query _,
		CancellationToken __
	)
	{
		if (contextAccessor.HttpContext is not {} httpContext)
		{
			return TypedResults.InternalServerError();
		}

		if (await userManager.GetUserAsync(httpContext.User) is not {} user)
		{
			return TypedResults.NotFound();
		}

		var passkeys = await userManager.GetPasskeysAsync(user);
		var mapped = passkeys.Select(p => new UserPasskey(
			Convert.ToBase64String(p.CredentialId),
			passkeyMetadata.GetDisplayName(p),
			p.CreatedAt)
		).ToList();

		return TypedResults.Ok(mapped);
	}

	public sealed record Query;

	public sealed record UserPasskey(string Id, string Name, DateTimeOffset CreationDate);
}