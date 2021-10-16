#nullable enable

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
    /// <returns>Whether the ID matches the logged in user ID</returns>
    public static bool IsUserSameAsLoggedIn(this ClaimsPrincipal principal, long userId) 
        => principal.GetNumericId() == userId;

    /// <summary>
    /// Get current user's ID automatically cast to `long?`
    /// </summary>
    /// <param name="principal">ClaimsPrincipal of the logged-in user</param>
    /// <returns>The ID of the user as `long` or `null` if user is not logged-in</returns>
    public static long? GetNumericId(this ClaimsPrincipal principal)
    {
        var user = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        var castResult = long.TryParse(user, out var userId);
        return castResult ? userId : null;
    }

    /// <summary>
    /// Get current user's username
    /// </summary>
    /// <param name="principal">ClaimsPrincipal of the logged-in user</param>
    /// <returns>The username of currently logged-in user or `null` if user isn't logged in</returns>
    public static string? GetUsername(this ClaimsPrincipal principal)
        => principal.FindFirstValue(ClaimTypes.Name);

    /// <summary>
    /// Check if the user is a staff member based on their roles
    /// </summary>
    /// <param name="principal">ClaimsPrincipal of the logged-in user</param>
    /// <returns>True if user is logged in and a staff member, false otherwise</returns>
    public static bool IsStaff(this ClaimsPrincipal principal)
    {
        var str = principal.FindFirstValue(OgmaClaimsPrincipalFactory.ClaimTypes.IsStaff);
        return bool.TryParse(str, out var isStaff) && isStaff;
    }
}