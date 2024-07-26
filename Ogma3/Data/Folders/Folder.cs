using Ogma3.Data.Bases;
using Ogma3.Data.Clubs;
using Ogma3.Data.Stories;

namespace Ogma3.Data.Folders;

public class Folder : BaseModel
{
	public string Name { get; set; } = null!;
	public string Slug { get; set; } = null!;
	public string? Description { get; set; }

	public Club Club { get; set; } = null!;
	public long ClubId { get; set; }

	public Folder? ParentFolder { get; set; }
	public long? ParentFolderId { get; set; }

	public ICollection<Folder> ChildFolders { get; set; } = null!;
	public ICollection<Story> Stories { get; set; } = null!;

	public int StoriesCount { get; set; }
	public EClubMemberRoles AccessLevel { get; set; }
}