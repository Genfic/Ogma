using System.ComponentModel.DataAnnotations;

namespace Ogma3.Data.Models
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