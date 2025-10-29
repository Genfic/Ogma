using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using Ogma3.Data;

namespace Ogma3.Infrastructure.Extensions;

public static class ClaimsPrincipalEx
{

	/// <param name="principal">ClaimsPrincipal of the logged-in user</param>
	extension(ClaimsPrincipal principal)
	{
		/// <summary>
		/// Check if the currently logged-in user's ID is the same as supplied ID
		/// </summary>
		/// <param name="userId">ID to check against</param>
		/// <returns>Whether the ID matches the logged-in user ID</returns>
		public bool IsUserSameAsLoggedIn(long userId)
			=> principal.GetNumericId() == userId;

		/// <summary>
		/// Get the current user's ID automatically cast to `long?`
		/// </summary>
		/// <returns>The ID of the user as `long` or `null` if the user is not logged in</returns>
		public long? GetNumericId()
		{
			var user = principal.FindFirstValue(ClaimTypes.NameIdentifier);
			return long.TryParse(user, out var userId) ? userId : null;
		}

		/// <summary>
		/// Get the current user's username
		/// </summary>
		/// <returns>The username of the currently logged-in user or `null` if the user isn't logged in</returns>
		public string? GetUsername()
			=> principal.FindFirstValue(ClaimTypes.Name);

		/// <summary>
		/// Check if the user is a staff member based on their roles
		/// </summary>
		/// <returns>True if the user is logged in and a staff member, false otherwise</returns>
		public bool IsStaff()
		{
			var str = principal.FindFirstValue(OgmaClaimsPrincipalFactory.ClaimTypes.IsStaff);
			return bool.TryParse(str, out var isStaff) && isStaff;
		}

		public bool HasAnyRole(params string[] roles)
			=> roles.Any(principal.IsInRole);

		public bool HasAllRoles(params string[] roles)
			=> roles.All(principal.IsInRole);

		public bool TryGetClaim(string claimType, [NotNullWhen(true)]out string? val)
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
}