using System.Globalization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Ogma3.Infrastructure.Extensions;

namespace Ogma3.Services.OAuthProviders.Discord;

public sealed class DiscordAuthenticationOptions : OAuthOptions
{
	public string? Prompt { get; set; }

	public DiscordAuthenticationOptions()
	{
		ClaimsIssuer = "Discord";
		CallbackPath = "/oauth/discord";
		AuthorizationEndpoint = "https://discord.com/api/oauth2/authorize";
		TokenEndpoint = "https://discord.com/api/oauth2/token";
		UserInformationEndpoint = "https://discord.com/api/users/@me";

		ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id");
		ClaimActions.MapJsonKey(ClaimTypes.Name, "username");
		ClaimActions.MapJsonKey(ClaimTypes.Email, "email");

		ClaimActions.MapCustomJson(ClaimTypes.Avatar, user =>
			string.Format(
				CultureInfo.InvariantCulture,
				"https://cdn.discordapp.com/avatars/{0}/{1}.png",
				user.GetString("id"),
				user.GetString("avatar")
			)
		);

		Scope.Add("identify");
	}
}