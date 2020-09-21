using System.Security.Claims;

namespace Utils.Extensions
{
    public static class ClaimsPrincipalEx
    {
        /// <summary>
        /// Check if the currently logged-in user's ID is the same as supplied ID
        /// </summary>
        /// <param name="principal">ClaimsPrincipal containing the logged-in user</param>
        /// <param name="userId">ID to check against</param>
        /// <returns>Whether the ID matches the logged in user ID</returns>
        public static bool IsUserSameAsLoggedIn(this ClaimsPrincipal principal, long userId) => principal.GetNumericId() == userId;

        /// <summary>
        /// Get current user's ID automatically cast to `long?`
        /// </summary>
        /// <param name="principal">ClaimsPrincipal containing the logged-in user</param>
        /// <returns>The ID of the user as `long` or `null` if user is not logged-in</returns>
        public static long? GetNumericId(this ClaimsPrincipal principal)
        {
            var user = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var castResult = long.TryParse(user, out var userId);
            return castResult ? userId : (long?) null;
        }
    }
}