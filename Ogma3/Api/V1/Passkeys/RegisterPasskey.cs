using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Ogma3.Data;
using Ogma3.Data.Users;

namespace Ogma3.Api.V1.Passkeys;

using ReturnType = Results<BadRequest<string>, BadRequest<IEnumerable<string>>, InternalServerError, Ok<RegisterPasskey.Response>, NotFound>;

[Handler]
[MapPost("api/passkeys/register")]
public static partial class RegisterPasskey
{
	[UsedImplicitly]
	public sealed record Query(string Credentials, string? Name);

	private static async ValueTask<ReturnType> Handle(
		Query query,
		IHttpContextAccessor contextAccessor,
		SignInManager<OgmaUser> signInManager,
		OgmaUserManager userManager,
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

		var attestationResult = await signInManager.PerformPasskeyAttestationAsync(query.Credentials);

		if (!attestationResult.Succeeded)
		{
			return TypedResults.BadRequest($"Error: {attestationResult.Failure?.Message}");
		}

		attestationResult.Passkey.Name = query.Name;

		var addResult = await userManager.AddOrUpdatePasskeyAsync(user, attestationResult.Passkey);

		if (!addResult.Succeeded)
		{
			return TypedResults.BadRequest(addResult.Errors.Select(e => $"[{e.Code}]: ${e.Description}"));
		}

		var passkey = attestationResult.Passkey;
		var response = new Response(Convert.ToBase64String(passkey.CredentialId), passkey.Name, passkey.CreatedAt);

		return TypedResults.Ok(response);
	}

	public sealed record Response(string Id, string? Name, DateTimeOffset CreationDate);
}