using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Ogma3.Infrastructure.Extensions;

namespace Ogma3.Services.OAuthProviders.Patreon;

public sealed class PatreonAuthenticationOptions : OAuthOptions
{
	public PatreonAuthenticationOptions()
	{
		ClaimsIssuer = "Patreon";
		CallbackPath = "/oauth/patreon";

		AuthorizationEndpoint = "https://www.patreon.com/oauth2/authorize";
		TokenEndpoint = "https://www.patreon.com/api/oauth2/token";
		UserInformationEndpoint = "https://www.patreon.com/api/oauth2/v2/identity";

		Scope.Add("identity");
		Scope.Add("identity[email]");

		ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id");
		ClaimActions.MapJsonSubKey(ClaimTypes.Email, "attributes", "email");
		ClaimActions.MapJsonSubKey(ClaimTypes.Avatar, "attributes", "image_url");

		Events.OnCreatingTicket = context => {
			var user = context.User;
			string? name = null;

			if (user.TryGetProperty("data", out var dataEl))
			{
				var attributes = dataEl.GetProperty("attributes");

				if (attributes.TryGetProperty("vanity", out var vanityEl) && vanityEl.ValueKind != JsonValueKind.Null)
				{
					name = vanityEl.GetString();
				}

				if (string.IsNullOrEmpty(name))
				{
					if (attributes.TryGetProperty("full_name", out var fullNameEl) && fullNameEl.ValueKind != JsonValueKind.Null)
					{
						name = fullNameEl.GetString();
					}
				}
			}

			if (string.IsNullOrEmpty(name))
			{
				return Task.CompletedTask;
			}

			var identity = (ClaimsIdentity?)context.Principal?.Identity;

			var existingClaim = identity?.FindFirst(ClaimTypes.Name);
			if (existingClaim is not null)
			{
				identity?.RemoveClaim(existingClaim);
			}

			identity?.AddClaim(new Claim(ClaimTypes.Name, name, ClaimValueTypes.String, context.Options.ClaimsIssuer));

			return Task.CompletedTask;
		};
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