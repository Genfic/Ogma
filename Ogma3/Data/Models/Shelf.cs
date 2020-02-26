using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Newtonsoft.Json;

namespace Ogma3.Data.Models
{
    public class Shelf : BaseModel
    {
        [Required]
        public string Name { get; set; }
        
        [Required]
        public User Owner { get; set; }
        
        [Required]
        public bool IsDefault { get; set; } = false;
        
        [Required]
        public bool IsPublic { get; set; } = false;
        
        // Stories
        [JsonIgnore]
        public virtual ICollection<ShelfStory> ShelfStories { get; set; }
        [NotMapped]
        public IEnumerable<Story> Stories =>
            ShelfStories == null
                ? new List<Story>()
                : ShelfStories.Select(ss => ss.Story).ToList();
    }
}