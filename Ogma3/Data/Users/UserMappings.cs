using System;
using System.Linq;
using System.Linq.Expressions;
using Ogma3.Data.Roles;
using Ogma3.Pages.Shared.Bars;

namespace Ogma3.Data.Users
{
    public static class UserMappings
    {
        public static Expression<Func<OgmaUser, ProfileBar>> ToProfileBar(long? uid) => u => new ProfileBar
        {
            Id = u.Id,
            UserName = u.UserName,
            Avatar = u.Avatar,
            Email = u.Email,
            Title = u.Title,
            LastActive = u.LastActive,
            RegistrationDate = u.RegistrationDate,
            FollowersCount = u.Followers.Count,
            BlogpostsCount = u.Blogposts.Count(b => b.PublicationDate != null),
            StoriesCount = u.Stories.Count(s => s.PublicationDate != null),
            IsBlockedBy = u.BlockedByUsers.Any(bu => bu.Id == uid),
            IsFollowedBy = u.Followers.Any(fu => fu.Id == uid),
            Roles = u.Roles.AsQueryable().Select(RoleMappings.ToRoleDto).ToList()
        };
    }
}