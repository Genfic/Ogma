using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using Ogma3.Data;

namespace Ogma3.Infrastructure.Extensions;

public static class ClaimsPrincipalEx
{
	/// <summary>
	/// Check if the currently logged-in user's ID is the same as supplied ID
	/// </summary>
	/// <param name="principal">ClaimsPrincipal of the logged-in user</param>
	/// <param name="userId">ID to check against</param>
	/// <returns>Whether the ID matches the logged-in user ID</returns>
	public static bool IsUserSameAsLoggedIn(this ClaimsPrincipal principal, long userId)
		=> principal.GetNumericId() == userId;

	/// <summary>
	/// Get the current user's ID automatically cast to `long?`
	/// </summary>
	/// <param name="principal">ClaimsPrincipal of the logged-in user</param>
	/// <returns>The ID of the user as `long` or `null` if the user is not logged in</returns>
	public static long? GetNumericId(this ClaimsPrincipal principal)
	{
		var user = principal.FindFirstValue(ClaimTypes.NameIdentifier);
		return long.TryParse(user, out var userId) ? userId : null;
	}

	/// <summary>
	/// Get the current user's username
	/// </summary>
	/// <param name="principal">ClaimsPrincipal of the logged-in user</param>
	/// <returns>The username of the currently logged-in user or `null` if the user isn't logged in</returns>
	public static string? GetUsername(this ClaimsPrincipal principal)
		=> principal.FindFirstValue(ClaimTypes.Name);

	/// <summary>
	/// Check if the user is a staff member based on their roles
	/// </summary>
	/// <param name="principal">ClaimsPrincipal of the logged-in user</param>
	/// <returns>True if the user is logged in and a staff member, false otherwise</returns>
	public static bool IsStaff(this ClaimsPrincipal principal)
	{
		var str = principal.FindFirstValue(OgmaClaimsPrincipalFactory.ClaimTypes.IsStaff);
		return bool.TryParse(str, out var isStaff) && isStaff;
	}

	public static bool HasAnyRole(this ClaimsPrincipal principal, params string[] roles)
		=> roles.Any(principal.IsInRole);

	public static bool HasAllRoles(this ClaimsPrincipal principal, params string[] roles)
		=> roles.All(principal.IsInRole);

	public static bool TryGetClaim(this ClaimsPrincipal principal, string claimType, [NotNullWhen(true)]out string? val)
	{
		if (principal.HasClaim(c => c.Type == claimType))
		{
			var claim = principal.FindFirstValue(claimType);
			if (claim is null)
			{
				val = null;
				return false;
			}

			val = claim;
			return true;
		}

		val = null;
		return false;
	}
}