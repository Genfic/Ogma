using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Ogma3.Data;
using Ogma3.Data.Users;

namespace Ogma3.Api.V1.Passkeys;

using ReturnType =
	Results<BadRequest<string>, BadRequest<IEnumerable<string>>, InternalServerError, Ok<RegisterPasskey.Response>, NotFound>;

[Handler]
[MapGroup<ApiGroup>]
[Authorize]
[MapPost("passkeys/register")]
public sealed partial class RegisterPasskey
	(IHttpContextAccessor contextAccessor, SignInManager<OgmaUser> signInManager, OgmaUserManager userManager)
{

	private async ValueTask<ReturnType> Handle(
		Query query,
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

	[UsedImplicitly]
	public sealed record Query(string Credentials, string? Name);

	public sealed record Response(string Id, string? Name, DateTimeOffset CreationDate);
}