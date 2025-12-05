namespace Ogma3.Services.OAuthProviders.Patreon;

public static class PatreonAuthenticationDefaults
{
	public const string AuthenticationScheme = "Patreon";

	public static readonly string DisplayName = "Patreon";

	public const string Issuer = "Patreon";

	public const string CallbackPath = "/oauth/patreon";

	public const string AuthorizationEndpoint = "https://www.patreon.com/oauth2/authorize";

	public const string TokenEndpoint = "https://www.patreon.com/api/oauth2/token";

	public const string UserInformationEndpoint = "https://www.patreon.com/api/oauth2/v2/identity";
}