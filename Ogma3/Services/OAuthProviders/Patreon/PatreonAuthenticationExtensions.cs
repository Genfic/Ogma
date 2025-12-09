using Microsoft.AspNetCore.Authentication;

namespace Ogma3.Services.OAuthProviders.Patreon;

public static class PatreonAuthenticationExtensions
{
	public static AuthenticationBuilder AddPatreon(this AuthenticationBuilder builder, Action<PatreonAuthenticationOptions> configuration)
		=> builder.AddOAuth<PatreonAuthenticationOptions, PatreonAuthenticationHandler>("Patreon", "Patreon", configuration);
}