namespace Ogma3.Data.Models
{
    public class FolderStory
    {
        public Folder Folder { get; set; }
        public long FolderId { get; set; }

        public Story Story { get; set; }
        public long StoryId { get; set; }
    }
}