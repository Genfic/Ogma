using Ogma3.Data.Stories;

namespace Ogma3.Data.Folders
{
    public class FolderStory
    {
        public Folder Folder { get; set; }
        public long FolderId { get; init; }
        public Story Story { get; set; }
        public long StoryId { get; init; }
    }
}