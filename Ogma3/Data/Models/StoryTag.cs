using System.ComponentModel.DataAnnotations;

namespace Ogma3.Data.Models
{
    public class StoryTag
    {
        [Required]
        public  Story Story { get; set; }
        [Required]
        public long StoryId { get; set; }

        [Required]
        public  Tag Tag { get; set; }
        [Required]
        public long TagId { get; set; }
    }
}