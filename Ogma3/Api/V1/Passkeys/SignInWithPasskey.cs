using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Ogma3.Data.Users;

namespace Ogma3.Api.V1.Passkeys;

using ReturnType = Results<Ok, UnauthorizedHttpResult>;

[Handler]
[MapPost("api/passkeys/signin")]
public static partial class SignInWithPasskey
{
	[UsedImplicitly]
	public sealed record Query(string Credentials);

	private static async ValueTask<ReturnType> Handle(Query request, SignInManager<OgmaUser> signInManager, CancellationToken _)
	{
		var res = await signInManager.PasskeySignInAsync(request.Credentials);

		return res.Succeeded
			? TypedResults.Ok()
			: TypedResults.Unauthorized();
	}
}