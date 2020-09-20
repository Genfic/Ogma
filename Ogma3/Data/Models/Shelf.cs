#nullable enable 

using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Newtonsoft.Json;

namespace Ogma3.Data.Models
{
    public class Shelf : BaseModel
    {
        [Required]
        [MinLength(CTConfig.CShelf.MinNameLength)]
        [MaxLength(CTConfig.CShelf.MaxNameLength)]
        public string Name { get; set; }

        [MaxLength(CTConfig.CShelf.MaxDescriptionLength)]
        public string? Description { get; set; } = "";
        
        [Required]
        public OgmaUser Owner { get; set; }
        
        [Required]
        [DefaultValue(false)]
        public bool IsDefault { get; set; } = false;

        [Required] 
        [DefaultValue(false)] 
        public bool IsPublic { get; set; } = false;

        [Required]
        [DefaultValue(false)] 
        public bool IsQuickAdd { get; set; } = false;
        
        [DefaultValue(null)]
        [MinLength(7)]
        [MaxLength(7)]
        public string? Color { get; set; }

        public Icon? Icon { get; set; }
        public long? IconId { get; set; }
        
        // Stories
        [JsonIgnore]
        public  ICollection<ShelfStory> ShelfStories { get; set; } = new List<ShelfStory>();
        [NotMapped]
        public IEnumerable<Story> Stories =>
            ShelfStories == null || ShelfStories.Count <= 0
                ? new List<Story>()
                : ShelfStories.Select(ss => ss.Story).ToList();
    }
}