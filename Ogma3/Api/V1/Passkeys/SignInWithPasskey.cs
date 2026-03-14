using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Ogma3.Data.Users;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Infrastructure.IResults;

namespace Ogma3.Api.V1.Passkeys;

using ReturnType = Results<Ok, UnauthorizedResult>;

[Handler]
[MapPost("api/passkeys/signin")]
public static partial class SignInWithPasskey
{
	[UsedImplicitly]
	public sealed record Query(string Credentials);

	private static async ValueTask<ReturnType> Handle(Query request, SignInManager<OgmaUser> signInManager, CancellationToken _)
	{
		var res = await signInManager.PasskeySignInAsync(request.Credentials);

		if (res.IsLockedOut || res.IsNotAllowed || !res.Succeeded)
		{
			return TypedResults.ProperUnauthorized();
		}

		return TypedResults.Ok();
	}
}