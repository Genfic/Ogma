using System.ComponentModel.DataAnnotations;

namespace Ogma3.Data.Models
{
    public class StoryTag
    {
        [Required]
        public  Story Story { get; set; }
        [Required]
        public int StoryId { get; set; }

        [Required]
        public  Tag Tag { get; set; }
        [Required]
        public int TagId { get; set; }
    }
}