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

		ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id");
		ClaimActions.MapJsonSubKey(ClaimTypes.Email, "user", "email");
		ClaimActions.MapJsonSubKey(ClaimTypes.Name, "user", "name");
		ClaimActions.MapJsonSubKey(ClaimTypes.Avatar, "user", "image_url");
	}
}