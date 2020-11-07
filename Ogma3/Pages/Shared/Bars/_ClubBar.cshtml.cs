using System;

namespace Ogma3.Pages.Shared.Bars
{
    public class ClubBar
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Hook { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        public DateTime CreationDate { get; set; }
        public int ClubMembersCount { get; set; }
        public int ThreadsCount { get; set; }
        public int StoriesCount { get; set; }
        public bool IsMember { get; set; }
        public long FounderId { get; set; }
    }
}