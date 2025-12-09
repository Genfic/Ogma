using Microsoft.AspNetCore.Authentication;

namespace Ogma3.Services.OAuthProviders.Tumblr;

public static class TumblrAuthenticationExtensions
{
	public static AuthenticationBuilder AddTumblr(this AuthenticationBuilder builder, Action<TumblrAuthenticationOptions> configuration)
		=> builder.AddOAuth<TumblrAuthenticationOptions, TumblrAuthenticationHandler>("Tumblr", "Tumblr", configuration);
}