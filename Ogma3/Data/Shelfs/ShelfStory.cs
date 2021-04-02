using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Ogma3.Data.Stories;

namespace Ogma3.Data.Shelfs
{
    public class ShelfStory
    {
        [JsonIgnore]
        [Required]
        public  Shelf Shelf { get; set; }
        [Required]
        public long ShelfId { get; set; }

        [JsonIgnore]
        [Required]
        public  Story Story { get; set; }
        [Required]
        public long StoryId { get; set; }
    }
}