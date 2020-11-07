using System.Collections.Generic;
using Ogma3.Pages.Shared;
using Ogma3.Pages.Shared.Minimals;

namespace Ogma3.Data.DTOs
{
    public class FolderDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ICollection<FolderMinimal> ChildFolders { get; set; }
        public int StoriesCount { get; set; }
    }
}