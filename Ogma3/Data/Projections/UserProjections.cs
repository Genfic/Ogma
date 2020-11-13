using System.Linq;
using Ogma3.Data.DTOs;
using Ogma3.Data.Models;
using Ogma3.Pages.Shared.Bars;

namespace Ogma3.Data.Projections
{
    public static class UserProjections
    {
        public static IQueryable<ProfileBar> ToProfileBar(this IQueryable<OgmaUser> source, long? userId)
        {
            return source.Select(u => new ProfileBar
            {
                Id = u.Id,
                UserName = u.UserName,
                Title = u.Title,
                Avatar = u.Avatar,
                Email = u.Email,
                RegistrationDate = u.RegistrationDate,
                LastActive = u.LastActive,
                StoriesCount = u.Stories.Count,
                BlogpostsCount = u.Blogposts.Count,
                FollowersCount = u.Followers.Count,
                IsBlockedBy = u.BlockedByUsers.Any(bu => bu.Id == userId),
                IsFollowedBy = u.Followers.Any(fu => fu.Id == userId),
                Roles = u.Roles.Select(r => new RoleDto
                {
                    Id = r.Id,
                    Name = r.Name,
                    IsStaff = r.IsStaff,
                    Order = (int) r.Order,
                    Color = r.Color
                })
            });
        }
    }
}