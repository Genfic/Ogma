using System;
using System.Linq;
using System.Linq.Expressions;
using Ogma3.Pages.Shared.Bars;

namespace Ogma3.Data.Clubs
{
    public static class ClubMappings
    {
        public static Expression<Func<Club, ClubBar>> ToClubBar(long userId) => c => new ClubBar
        {
            Id = c.Id,
            Name = c.Name,
            Slug = c.Slug,
            Hook = c.Hook,
            Description = c.Description,
            Icon = c.Icon,
            CreationDate = c.CreationDate,
            ThreadsCount = c.Threads.Count,
            ClubMembersCount = c.ClubMembers.Count,
            StoriesCount = c.Folders.Sum(f => f.StoriesCount),
            FounderId = c.ClubMembers.First(cm => cm.Role == EClubMemberRoles.Founder).MemberId,
            Role = c.ClubMembers.Any(cm => cm.MemberId == userId)
                ? c.ClubMembers.First(cm => cm.MemberId == userId).Role
                : null
        };
    }
}