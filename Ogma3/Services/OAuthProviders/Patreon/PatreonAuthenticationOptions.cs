using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;

namespace Ogma3.Services.OAuthProviders.Patreon;

public sealed class PatreonAuthenticationOptions : OAuthOptions
{
	public PatreonAuthenticationOptions()
	{
		ClaimsIssuer = PatreonAuthenticationDefaults.Issuer;
		CallbackPath = PatreonAuthenticationDefaults.CallbackPath;

		AuthorizationEndpoint = PatreonAuthenticationDefaults.AuthorizationEndpoint;
		TokenEndpoint = PatreonAuthenticationDefaults.TokenEndpoint;
		UserInformationEndpoint = PatreonAuthenticationDefaults.UserInformationEndpoint;

		Scope.Add("identity");
		Scope.Add("identity[email]");

		ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id");
		ClaimActions.MapJsonSubKey(ClaimTypes.Email, "attributes", "email");
		ClaimActions.MapJsonSubKey(ClaimTypes.GivenName, "attributes", "first_name");
		ClaimActions.MapJsonSubKey(ClaimTypes.Name, "attributes", "full_name");
		ClaimActions.MapJsonSubKey(ClaimTypes.Surname, "attributes", "last_name");
		ClaimActions.MapJsonSubKey(ClaimTypes.Webpage, "attributes", "url");
		ClaimActions.MapJsonSubKey(PatreonAuthenticationConstants.Claims.Avatar, "attributes", "image_url");
		ClaimActions.MapJsonSubKey(PatreonAuthenticationConstants.Claims.Vanity, "attributes", "vanity");
	}

	/// <summary>
	/// Gets the list of fields to retrieve from the user information endpoint.
	/// </summary>
	public ISet<string> Fields { get; } = new HashSet<string>
	{
		"first_name",
		"full_name",
		"last_name",
		"image_url",
		"url",
		"vanity",
		"email",
	};

	/// <summary>
	/// Gets the list of related data to include from the user information endpoint.
	/// </summary>
	public ISet<string> Includes { get; } = new HashSet<string>();
}