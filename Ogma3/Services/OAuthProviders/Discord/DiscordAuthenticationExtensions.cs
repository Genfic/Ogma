using Microsoft.AspNetCore.Authentication;

namespace Ogma3.Services.OAuthProviders.Discord;

public static class DiscordAuthenticationExtensions
{
	public static AuthenticationBuilder AddDiscord(this AuthenticationBuilder builder, Action<DiscordAuthenticationOptions> configuration)
		=> builder.AddOAuth<DiscordAuthenticationOptions, DiscordAuthenticationHandler>("Discord", "Discord", configuration);
}