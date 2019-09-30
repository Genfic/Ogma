using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Ogma3.Data
{
    public class OgmaClaimsPrincipalFactory : UserClaimsPrincipalFactory<User, IdentityRole>
    {
        public OgmaClaimsPrincipalFactory(
            UserManager<User> userManager, 
            RoleManager<IdentityRole> roleManager, 
            IOptions<IdentityOptions> options) 
            : base(userManager, roleManager, options)
        { }

        public override async Task<ClaimsPrincipal> CreateAsync(User user)
        {
            var principal = await base.CreateAsync(user);
            
            ((ClaimsIdentity)principal.Identity).AddClaims(new []
            {
                new Claim(OgmaClaimTypes.Avatar, user.Avatar),
                new Claim(OgmaClaimTypes.Title, user.Title),
            });

            return principal;
        }
    }
}