using System.ComponentModel.DataAnnotations;

namespace Ogma3.Data.Models
{
    public class StoryTag
    {
        [Required]
        public  Story Story { get; set; }
        public long StoryId { get; set; }

        [Required]
        public  Tag Tag { get; set; }
        public long TagId { get; set; }
    }
}