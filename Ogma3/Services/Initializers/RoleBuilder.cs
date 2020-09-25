using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Ogma3.Data.Models;

namespace Ogma3.Services.Initializers
{
    public class RoleBuilder
    {
        private OgmaRole Role { get; }
        private readonly List<Claim> _claims;
        private readonly RoleManager<OgmaRole> _roleManager;

        public RoleBuilder(OgmaRole role, RoleManager<OgmaRole> roleManager)
        {
            Role = role;
            _roleManager = roleManager;
            _claims = new List<Claim>();
        }

        public RoleBuilder AddClaim(string type, string value)
        {
            _claims.Add(new Claim(type, value));
            return this;
        }

        public async Task Build()
        {
            OgmaRole role;
            if (!await _roleManager.RoleExistsAsync(Role.Name))
            {
                role = Role;
                await _roleManager.CreateAsync(Role);
            }
            else
            {
                role = await _roleManager.FindByNameAsync(Role.Name);
            }
            
            foreach (var claim in _claims)
            {
                var claims = await _roleManager.GetClaimsAsync(role);
                if (claims.Any(c => c.Type == claim.Type && c.Value == claim.Value)) continue;

                await _roleManager.AddClaimAsync(role, claim);
            }
        }
        
    }
}