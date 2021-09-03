using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Ogma3.Data.Roles;
using Ogma3.Data.Users;
using Ogma3.Infrastructure.Constants;

namespace Ogma3.Data
{
    public class OgmaClaimsPrincipalFactory : UserClaimsPrincipalFactory<OgmaUser, OgmaRole>
    {
        public OgmaClaimsPrincipalFactory(
            UserManager<OgmaUser> userManager, 
            RoleManager<OgmaRole> roleManager, 
            IOptions<IdentityOptions> options) 
            : base(userManager, roleManager, options)
        { }

        public override async Task<ClaimsPrincipal> CreateAsync(OgmaUser user)
        {
            var principal = await base.CreateAsync(user);
            
            ((ClaimsIdentity)principal.Identity)?.AddClaims(new Claim[]
            {
                new (ClaimTypes.Avatar, user.Avatar ?? string.Empty),
                new (ClaimTypes.Title, user.Title ?? string.Empty),
            });

            return principal;
        }
        
        public static class ClaimTypes
        {
            public const string Avatar = "Avatar";
            public const string Title  = "Title";
        }
    }
}