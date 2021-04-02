using System;
using System.Collections.Generic;

namespace Ogma3.Data.Users
{
    public sealed record UserDetailsDto
    {
        public long Id { get; init; }
        public string Name { get; init; }
        public string Email { get; init; }
        public string? Title { get; init; }
        public string? Bio { get; init; }
        public string? Avatar { get; init; }
        public DateTime RegistrationDate { get; init; }
        public DateTime LastActive { get; init; }
        public int StoriesCount { get; init; }
        public int BlogpostsCount { get; init; }
        public IEnumerable<string> RoleNames { get; init; }
        public DateTime? BannedUntil { get; init; }
        public DateTime? MutedUntil { get; init; }
    }
}