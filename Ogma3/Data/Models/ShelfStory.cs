using System.Text.Json.Serialization;

namespace Ogma3.Data.Models
{
    public class ShelfStory
    {
        [JsonIgnore]
        public  Shelf Shelf { get; set; }
        public int ShelfId { get; set; }

        [JsonIgnore]
        public  Story Story { get; set; }
        public int StoryId { get; set; }
    }
}