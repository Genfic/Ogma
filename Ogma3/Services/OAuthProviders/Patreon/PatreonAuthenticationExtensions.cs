using Microsoft.AspNetCore.Authentication;

namespace Ogma3.Services.OAuthProviders.Patreon;

public static class PatreonAuthenticationExtensions
{
	public static AuthenticationBuilder AddPatreon(this AuthenticationBuilder builder, Action<PatreonAuthenticationOptions> configuration)
	{
		return builder.AddOAuth<PatreonAuthenticationOptions, PatreonAuthenticationHandler>(
			PatreonAuthenticationDefaults.AuthenticationScheme,
			PatreonAuthenticationDefaults.DisplayName,
			configuration
		);
	}
}