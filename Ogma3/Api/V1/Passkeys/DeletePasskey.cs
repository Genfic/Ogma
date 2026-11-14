using System.Buffers.Text;
using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Ogma3.Data;
using Ogma3.Data.SecurityTransactions;
using Ogma3.Data.Users;
using Ogma3.Services.Security;
namespace Ogma3.Api.V1.Passkeys;

using ReturnType =
	Results<Ok, NotFound, InternalServerError, ProblemHttpResult>;

[Handler]
[MapGroup<ApiGroup>]
[Authorize]
[MapDelete("passkeys/delete")]
public sealed partial class DeletePasskey
(
	OgmaUserManager userManager,
	IHttpContextAccessor contextAccessor,
	SignInManager<OgmaUser> signInManager,
	SecurityAuthorizationService securityAuth,
	LinkGenerator linkGenerator,
	IAntiforgery antiforgery,
	ILogger<DeletePasskey.Handler> logger)
{

	private async ValueTask<ReturnType> Handle(
		Query request,
		CancellationToken _
	)
	{
		if (contextAccessor.HttpContext is not {} httpContext)
		{
			return TypedResults.InternalServerError();
		}

		// The antiforgery middleware only validates POST/PUT/PATCH, so DELETE requests
		// must be validated explicitly
		try
		{
			await antiforgery.ValidateRequestAsync(httpContext);
		}
		catch (AntiforgeryValidationException e)
		{
			logger.LogWarning("Passkey deletion rejected: invalid antiforgery token ({Message})", e.Message);
			return TypedResults.Problem(title: "Antiforgery token invalid", statusCode: StatusCodes.Status400BadRequest);
		}

		if (await userManager.GetUserAsync(httpContext.User) is not {} user)
		{
			return TypedResults.NotFound();
		}

		var credentialId = System.Convert.FromBase64String(request.Id);

		// Removing a credential requires fresh account proof and, under a per-user row lock,
		// must leave at least one usable primary login path
		if (!await securityAuth.HasVerifiedGrantAsync(user, SecurityOperation.RemovePasskey, request.Id))
		{
			var txn = await securityAuth.BeginAsync(user, SecurityOperation.RemovePasskey, request.Id);
			return ReauthRequired.Problem(txn, linkGenerator, httpContext);
		}

		await using var dbTx = await securityAuth.BeginCredentialMutationAsync(user.Id);

		// Consume the grant inside the same transaction as the mutation
		if (!await securityAuth.ConsumeAsync(user, SecurityOperation.RemovePasskey, request.Id))
		{
			await dbTx.RollbackAsync();
			var txn = await securityAuth.BeginAsync(user, SecurityOperation.RemovePasskey, request.Id);
			return ReauthRequired.Problem(txn, linkGenerator, httpContext);
		}

		// Re-read authoritative state inside the lock
		if (await userManager.GetUserAsync(httpContext.User) is not {} freshUser)
		{
			await dbTx.RollbackAsync();
			return TypedResults.NotFound();
		}

		var passkeys = await userManager.GetPasskeysAsync(freshUser);
		if (!passkeys.Any(p => p.CredentialId.AsSpan().SequenceEqual(credentialId)))
		{
			await dbTx.RollbackAsync();
			return TypedResults.NotFound();
		}

		var logins = await userManager.GetLoginsAsync(freshUser);
		if (!LoginPathPolicy.HasUsablePrimaryLoginPath(
			    passkeys.Count - 1,
			    hasPassword: freshUser.PasswordHash is not null,
			    externalLoginCount: logins.Count))
		{
			await dbTx.RollbackAsync();
			return TypedResults.Problem(
				title: "Last primary login path",
				statusCode: StatusCodes.Status409Conflict,
				detail: "You cannot remove your only usable login method.");
		}

		var result = await userManager.RemovePasskeyAsync(freshUser, credentialId);

		if (!result.Succeeded)
		{
			await dbTx.RollbackAsync();
			logger.LogWarning("Could not remove passkey {PasskeyId} for user {UserId}", request.Id, freshUser.Id);
			return TypedResults.InternalServerError();
		}

		await dbTx.CommitAsync();

		// Bump the security stamp so other sessions are revoked within the stamp revalidation
		// interval, then renew the current session's cookie with the new stamp
		await userManager.UpdateSecurityStampAsync(freshUser);
		await securityAuth.InvalidateForUserAsync(freshUser.Id);
		await signInManager.RefreshSignInAsync(freshUser);

		logger.LogInformation("Removed passkey {PasskeyId} for user {UserId}", request.Id, freshUser.Id);

		return TypedResults.Ok();
	}

	[UsedImplicitly]
	public sealed record Query([FromQuery] string Id);
}
