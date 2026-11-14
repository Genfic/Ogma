using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;

namespace Ogma3.Services.Security;

/// <summary>
/// Custom cookie authentication events for the Identity application cookie.
///
/// Mints a stable, unpredictable per-session identifier (`sid`) into the cookie's
/// <see cref="AuthenticationProperties"/>: kept across ordinary refreshes (sliding renewal,
/// <see cref="Identity.SignInManager{OgmaUser}.RefreshSignInAsync">RefreshSignInAsync</see>,
/// security-stamp revalidation) and regenerated only for genuinely new sign-ins.
///
/// A `sid` lives in <see cref="AuthenticationProperties"/> rather than in claims because
/// <see cref="SecurityStampValidator"/> rebuilds the principal on stamp verification and would
/// drop claims that aren't re-derived from the user store; properties survive principal replacement.
/// </summary>
public sealed class OgmaCookieAuthenticationEvents : CookieAuthenticationEvents
{
	public const string SessionIdKey = "ogma.sid";

	public override Task SigningIn(CookieSigningInContext context)
	{
		SidHelper.EnsureSessionId(context.Properties);
		return base.SigningIn(context);
	}

	public override async Task ValidatePrincipal(CookieValidatePrincipalContext context)
	{
		// Pre-deployment cookies (and any cookie that lost its sid) get a fresh session
		// identifier on first validation, and the ticket is renewed to persist it
		if (SidHelper.EnsureSessionId(context.Properties))
		{
			context.ShouldRenew = true;
		}

		// EventsType replaces Identity's default wiring, so stamp validation must be chained explicitly
		await SecurityStampValidator.ValidatePrincipalAsync(context);
	}
}

public static class SidHelper
{
	/// <summary>
	/// Adds a new session identifier to the properties if none is present.
	/// </summary>
	/// <returns>True if a new sid was generated (the ticket should be renewed).</returns>
	public static bool EnsureSessionId(AuthenticationProperties? properties)
	{
		if (properties is null) return false;
		if (properties.Items.TryGetValue(OgmaCookieAuthenticationEvents.SessionIdKey, out var existing) &&
			!string.IsNullOrEmpty(existing))
		{
			return false;
		}

		properties.Items[OgmaCookieAuthenticationEvents.SessionIdKey] = NewSessionId();
		return true;
	}

	public static string NewSessionId()
	{
		// 32 bytes of entropy, base64url, no padding — 43 characters
		return Base64UrlEncode(RandomNumberGenerator.GetBytes(32));
	}

	public static string Base64UrlEncode(byte[] bytes)
		=> Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');

	public static string? GetSessionId(AuthenticationProperties? properties)
		=> properties is not null &&
		   properties.Items.TryGetValue(OgmaCookieAuthenticationEvents.SessionIdKey, out var sid) &&
		   !string.IsNullOrEmpty(sid)
			? sid
			: null;
}
