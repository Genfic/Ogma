using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Ogma3.Data.Models;
using Utils;

namespace Ogma3.Data
{
    public class OgmaClaimsPrincipalFactory : UserClaimsPrincipalFactory<OgmaUser, Role>
    {
        public OgmaClaimsPrincipalFactory(
            UserManager<OgmaUser> userManager, 
            RoleManager<Role> roleManager, 
            IOptions<IdentityOptions> options) 
            : base(userManager, roleManager, options)
        { }

        public override async Task<ClaimsPrincipal> CreateAsync(OgmaUser user)
        {
            var principal = await base.CreateAsync(user);
            
            ((ClaimsIdentity)principal.Identity).AddClaims(new []
            {
                new Claim(OgmaClaimTypes.Avatar, user.Avatar ?? Lorem.Gravatar(user.Email)),
                new Claim(OgmaClaimTypes.Title, user.Title ?? ""),
            });

            return principal;
        }
    }
}