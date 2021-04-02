using System.Collections.Generic;
using Ogma3.Data.Bases;
using Ogma3.Data.Clubs;
using Ogma3.Data.Stories;

namespace Ogma3.Data.Folders
{
    public class Folder : BaseModel
    {
        public string Name { get; set; }
        public string Slug { get; set; }
        public string? Description { get; set; }
        
        public Club Club { get; set; }
        public long ClubId { get; set; }

        public Folder? ParentFolder { get; set; }
        public long? ParentFolderId { get; set; }

        public ICollection<Folder> ChildFolders { get; set; }
        public ICollection<Story> Stories { get; set; }

        public int StoriesCount { get; set; }
        public EClubMemberRoles AccessLevel { get; set; }
    }
}