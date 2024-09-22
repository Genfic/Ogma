#nullable disable

using Ogma3.Data.Stories;
using Ogma3.Data.Users;

namespace Ogma3.Data.Folders;

public sealed class FolderStory
{
	public Folder Folder { get; set; }
	public long FolderId { get; init; }
	public Story Story { get; set; }
	public long StoryId { get; init; }
	public DateTimeOffset Added { get; init; }
	public OgmaUser AddedBy { get; set; }
	public long AddedById { get; init; }
}