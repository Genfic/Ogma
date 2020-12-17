using System.Collections.Generic;
using Ogma3.Data.Enums;
using Ogma3.Pages.Shared.Minimals;

namespace Ogma3.Data.DTOs
{
    public class FolderDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public string Description { get; set; }
        public IEnumerable<FolderMinimal> ChildFolders { get; set; }
        public int StoriesCount { get; set; }
        public EClubMemberRoles AccessLevel { get; set; }
    }
}