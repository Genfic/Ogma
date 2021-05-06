using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Roles;
using Ogma3.Infrastructure.Extensions;

namespace Ogma3.Pages.Shared.Components.ProfileBar
{
    public class ProfileBarViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public ProfileBarViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public class ProfileBar
        {
            public long Id { get; init; }
            public string UserName { get; init; }
            public string Title { get; init; }
            public string Avatar { get; init; }
            public string Email { get; init; }
            public DateTime RegistrationDate { get; init; }
            public DateTime LastActive { get; init; }
            public IEnumerable<RoleDto> Roles { get; init; }
            public int StoriesCount { get; init; }
            public int BlogpostsCount { get; init; }
            public int FollowersCount { get; init; }
            public bool IsBlockedBy { get; init; }
            public bool IsFollowedBy { get; init; }
        }

        public async Task<IViewComponentResult> InvokeAsync(string name)
        {
            var uid = UserClaimsPrincipal.GetNumericId();
            
            var user = await _context.Users
                .Where(u => u.NormalizedUserName == name.Normalize().ToUpperInvariant())
                .Select(u => new ProfileBar
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Avatar = u.Avatar,
                    Email = u.Email,
                    Title = u.Title,
                    LastActive = u.LastActive,
                    RegistrationDate = u.RegistrationDate,
                    FollowersCount = u.Followers.Count,
                    BlogpostsCount = u.Blogposts.Count(b => b.IsPublished),
                    StoriesCount = u.Stories.Count(s => s.IsPublished),
                    IsBlockedBy = u.BlockedByUsers.Any(bu => bu.Id == uid),
                    IsFollowedBy = u.Followers.Any(fu => fu.Id == uid),
                    Roles = u.Roles.AsQueryable().Select(RoleMappings.ToRoleDto).ToList()
                })
                .FirstOrDefaultAsync();
            
            return View(nameof(ProfileBarViewComponent), user);
        }
    }
}