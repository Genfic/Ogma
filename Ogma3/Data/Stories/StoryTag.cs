using Ogma3.Data.Tags;

namespace Ogma3.Data.Stories
{
    public class StoryTag
    {
        public  Story Story { get; set; }
        public long StoryId { get; set; }

        public  Tag Tag { get; set; }
        public long TagId { get; set; }
    }
}