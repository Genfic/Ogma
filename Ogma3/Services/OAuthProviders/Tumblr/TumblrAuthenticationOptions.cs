using System.Security.Claims;
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

		var userName = userElement.GetProperty("name").GetString();
		string? stableIdentifier = null;

		// Extract the stable identifier (Primary Blog UUID)
		if (userElement.TryGetProperty("blogs", out var blogs) && blogs.GetArrayLength() > 0)
		{
			var primaryBlog = blogs
				.EnumerateArray()
				.FirstOrDefault(b => b.TryGetProperty("primary", out var primary) && primary.GetBoolean());

			if (primaryBlog.TryGetProperty("uuid", out var uuidElement))
			{
				stableIdentifier = uuidElement.GetString();
			}
		}

		if (string.IsNullOrEmpty(stableIdentifier))
		{
			stableIdentifier = userName;
		}

		if (string.IsNullOrEmpty(stableIdentifier))
		{
			return Task.CompletedTask;
		}

		var identity = (ClaimsIdentity?)context.Principal?.Identity;

		var existingClaim = identity?.FindFirst(ClaimTypes.NameIdentifier);
		if (existingClaim is not null)
		{
			identity?.RemoveClaim(existingClaim);
		}

		identity?.AddClaim(new Claim(ClaimTypes.NameIdentifier, stableIdentifier, ClaimValueTypes.String, context.Options.ClaimsIssuer));

		return Task.CompletedTask;
	}
}