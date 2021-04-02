using System.ComponentModel.DataAnnotations;
using Ogma3.Data.Stories;

namespace Ogma3.Data.Folders
{
    public class FolderStory
    {
        [Required]
        public Folder Folder { get; set; }
        public long FolderId { get; set; }

        [Required]
        public Story Story { get; set; }
        public long StoryId { get; set; }
    }
}