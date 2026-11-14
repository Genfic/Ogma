namespace Ogma3.Services.Security;

/// <summary>
/// Decides whether an account would retain at least one usable primary login path
/// after a credential removal. TOTP, safety PIN, contact email, and second-factor
/// recovery codes are not primary login methods.
/// </summary>
public static class LoginPathPolicy
{
	/// <param name="passkeyCount">Number of enrolled passkeys after the proposed removal.</param>
	/// <param name="hasPassword">Whether a password hash is present after the proposed removal.</param>
	/// <param name="externalLoginCount">Number of linked external logins after the proposed removal.</param>
	public static bool HasUsablePrimaryLoginPath(int passkeyCount, bool hasPassword, int externalLoginCount)
		=> passkeyCount > 0 || hasPassword || externalLoginCount > 0;
}

public readonly record struct CredentialCounts(int Passkeys, bool HasPassword, int ExternalLogins);
