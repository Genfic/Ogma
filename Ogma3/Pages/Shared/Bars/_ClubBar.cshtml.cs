using System;
using Ogma3.Data.Clubs;

namespace Ogma3.Pages.Shared.Bars
{
    public class ClubBar
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public string Hook { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        public DateTime CreationDate { get; set; }
        public int ClubMembersCount { get; set; }
        public int ThreadsCount { get; set; }
        public int StoriesCount { get; set; }
        public bool IsMember => Role is not null;
        public long FounderId { get; set; }
        public EClubMemberRoles? Role { get; set; }
    }
}