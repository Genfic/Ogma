using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Ogma3.Data;

namespace Ogma3.Api.V1.Passkeys;

using ReturnType = Results<Ok, NotFound, InternalServerError>;

[Handler]
[MapDelete("api/passkeys/delete")]
public sealed partial class DeletePasskey(
	OgmaUserManager userManager,
	IHttpContextAccessor contextAccessor,
	ILogger<DeletePasskey> logger)
{
	[UsedImplicitly]
	public sealed record Query([FromQuery] string Id);

	private async ValueTask<ReturnType> Handle(
		Query request,
		CancellationToken _
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

		var passkeyId = Convert.FromBase64String(request.Id);

		var result = await userManager.RemovePasskeyAsync(user, passkeyId);

		if (result.Succeeded)
		{
			return TypedResults.Ok();
		}

		logger.LogWarning("Could not remove passkey {PasskeyId} for user {UserId}", request.Id, user.Id);
		return TypedResults.InternalServerError();
	}
}