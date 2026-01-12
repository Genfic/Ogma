using AutoDbSetGenerators;
using Ogma3.Data.Stories;
using Ogma3.Data.Users;

namespace Ogma3.Data.Folders;

[AutoDbSet]
public sealed class FolderStory
{
	public Folder Folder { get; set; } = null!;
	public long FolderId { get; init; }
	public Story Story { get; set; } = null!;
	public long StoryId { get; init; }
	public DateTimeOffset Added { get; init; }
	public OgmaUser AddedBy { get; set; } = null!;
	public long AddedById { get; init; }
}