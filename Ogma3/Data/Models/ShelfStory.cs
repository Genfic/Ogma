namespace Ogma3.Data.Models
{
    public class ShelfStory
    {
        public  Shelf Shelf { get; set; }
        public int ShelfId { get; set; }

        public  Story Story { get; set; }
        public int StoryId { get; set; }
    }
}