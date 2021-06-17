using System;
using System.Collections.Generic;
using Ogma3.Data.Roles;

namespace Ogma3.Pages.Shared.Bars
{
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
}