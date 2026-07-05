using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Ogma3.Data;
using Ogma3.Data.Users;

namespace Ogma3.Api.V1.Passkeys;

using ReturnType = Results<ContentHttpResult, InternalServerError, NotFound>;

[Handler]
[MapGroup<ApiGroup>]
[MapGet("passkeys/request-options")]
public sealed partial class GetPasskeyRequestOptions(SignInManager<OgmaUser> signInManager, OgmaUserManager userManager)
{
	internal static void CustomizeEndpoint(RouteHandlerBuilder endpoint)
		=> endpoint
			.Produces<string>(contentType: "application/json", statusCode: 200)
			.WithSummary("Returns WebAuthn creation options JSON")
			.WithDescription("Returns PublicKeyCredentialCreationOptionsJSON as defined by the WebAuthn spec.")
			.DisableAntiforgery();

	private async ValueTask<ReturnType> Handle(
		Query request,
		CancellationToken _
	)
	{
		var user = string.IsNullOrWhiteSpace(request.Username) ? null : await userManager.FindByNameAsync(request.Username);

		var options = await signInManager.MakePasskeyRequestOptionsAsync(user);

		return TypedResults.Content(options, "application/json", statusCode: 200);
	}

	[UsedImplicitly]
	public sealed record Query(string? Username);
}