using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Ogma3.Infrastructure.Extensions;

namespace Ogma3.Services.OAuthProviders.Tumblr;

public sealed class TumblrAuthenticationOptions : OAuthOptions
{
	public TumblrAuthenticationOptions()
	{
		ClaimsIssuer = "Tumblr";
		CallbackPath = "/oauth/tumblr"; // BUG: Something fucky-wucky going on here, can't find this path, apparently

		AuthorizationEndpoint = "https://www.tumblr.com/oauth2/authorize";
		TokenEndpoint = "https://api.tumblr.com/v2/oauth2/token";
		UserInformationEndpoint = "https://api.tumblr.com/v2/user/info";

		Scope.Add("basic");

		ClaimActions.MapJsonSubKey(ClaimTypes.Email, "user", "email");
		ClaimActions.MapJsonSubKey(ClaimTypes.Name, "user", "name");
		ClaimActions.MapJsonSubKey(ClaimTypes.Avatar, "user", "image_url");

		Events.OnCreatingTicket = OnCreatingTicket;
	}

	private static Task OnCreatingTicket(OAuthCreatingTicketContext context)
	{
		var userElement = context.User.GetProperty("response").GetProperty("user");
		var primaryBlog = userElement.TryGetProperty("blogs", out var blogs) && blogs.ValueKind == JsonValueKind.Array
			? blogs.EnumerateArray().FirstOrDefault(blog =>
				blog.ValueKind == JsonValueKind.Object
				&& blog.TryGetProperty("primary", out var primary)
				&& primary.ValueKind == JsonValueKind.True)
			: default;

		var stableIdentifier = primaryBlog.ValueKind == JsonValueKind.Object
			&& primaryBlog.TryGetProperty("uuid", out var uuidElement)
			&& uuidElement.ValueKind == JsonValueKind.String
			? uuidElement.GetString()
			: null;

		if (string.IsNullOrWhiteSpace(stableIdentifier))
		{
			throw new AuthenticationFailureException("Tumblr did not provide the stable identifier for the primary blog.");
		}

		if (context.Principal?.Identity is not ClaimsIdentity identity)
		{
			throw new AuthenticationFailureException("Tumblr authentication did not produce a claims identity.");
		}

		var existingClaim = identity.FindFirst(ClaimTypes.NameIdentifier);
		if (existingClaim is not null)
		{
			identity.RemoveClaim(existingClaim);
		}

		identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, stableIdentifier, ClaimValueTypes.String, context.Options.ClaimsIssuer));

		return Task.CompletedTask;
	}
}