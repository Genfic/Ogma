using System.Security.Claims;

namespace Utils.Extensions
{
    public static class ClaimsPrincipalEx
    {
        public static bool IsUserSameAsLoggedIn(this ClaimsPrincipal principal, long userId)
        {
            return principal.Identity.IsAuthenticated &&
                   principal.FindFirst(ClaimTypes.NameIdentifier)?.Value == userId.ToString();
        }
    }
}