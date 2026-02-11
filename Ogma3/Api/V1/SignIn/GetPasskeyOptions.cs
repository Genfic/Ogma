using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Ogma3.Data;
using Ogma3.Data.Users;
using SimpleBase;

namespace Ogma3.Api.V1.SignIn;

using ReturnType = Results<ContentHttpResult, InternalServerError, NotFound>;

[Handler]
[MapGet("api/signin/passkey-options")]
public static partial class GetPasskeyOptions
{
	internal static void CustomizeEndpoint(RouteHandlerBuilder endpoint) => endpoint
		.Produces<string>(contentType: "application/json", statusCode: 200)
		.WithSummary("Returns WebAuthn creation options JSON")
		.WithDescription("Returns PublicKeyCredentialCreationOptionsJSON as defined by the WebAuthn spec.")
		.DisableAntiforgery();

	[UsedImplicitly]
	public sealed record Query;

	private static async ValueTask<ReturnType> Handle(
		Query _,
		IHttpContextAccessor contextAccessor,
		SignInManager<OgmaUser> signInManager,
		OgmaUserManager userManager,
		CancellationToken ct
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

		var userId = await userManager.GetUserIdAsync(user);
		var userName = await userManager.GetUserNameAsync(user) ?? GetRandomName();

		var options = await signInManager.MakePasskeyCreationOptionsAsync(new()
		{
			Id = userId,
			Name = userName,
			DisplayName = userName,
		});

		return TypedResults.Content(options, contentType: "application/json", statusCode: 200);
	}

	private static string GetRandomName()
	{
		var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
		var bytes = BitConverter.GetBytes(timestamp);
		var encoded = Base32.Crockford.Encode(bytes);
		return $"User_{encoded}";
	}
}