namespace Ogma3.Data.Models
{
    public class StoryTag
    {
        public  Story Story { get; set; }
        public long StoryId { get; set; }

        public  Tag Tag { get; set; }
        public long TagId { get; set; }
    }
}