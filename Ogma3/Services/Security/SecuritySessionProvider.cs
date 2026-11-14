using System.Security.Claims;
using Immediate.Injections.Shared;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Ogma3.Infrastructure.Extensions;

namespace Ogma3.Services.Security;

/// <summary>
/// Resolves the application session identifier (`sid`) from the current request's
/// application authentication cookie. The sid is stable across ordinary refreshes and
/// changes only for genuinely new sign-ins.
/// </summary>
[RegisterScoped]
public sealed class SecuritySessionProvider(IHttpContextAccessor httpContextAccessor) : ISecuritySessionProvider
{
	public async Task<string?> GetSessionIdAsync()
	{
		if (httpContextAccessor.HttpContext is not { } http) return null;

		var result = await http.AuthenticateAsync(IdentityConstants.ApplicationScheme);
		if (!result.Succeeded) return null;

		return SidHelper.GetSessionId(result.Properties);
	}

	public async Task<long?> GetUserIdAsync()
		=> httpContextAccessor.HttpContext?.User.GetNumericId();
}

public interface ISecuritySessionProvider
{
	/// <summary>Identifier of the current application session, or null when not authenticated.</summary>
	Task<string?> GetSessionIdAsync();

	/// <summary>Database ID of the currently authenticated user, or null when not authenticated.</summary>
	Task<long?> GetUserIdAsync();
}
