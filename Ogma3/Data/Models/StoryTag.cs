namespace Ogma3.Data.Models
{
    public class StoryTag
    {
        public  Story Story { get; set; }
        public int StoryId { get; set; }

        public  Tag Tag { get; set; }
        public int TagId { get; set; }
    }
}