using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ogma3.Data.Models
{
    public class Shelf : BaseModel
    {
        [MinLength(CTConfig.CShelf.MinNameLength)]
        public string Name { get; set; }
        
        public string Description { get; set; }
        
        public OgmaUser Owner { get; set; }
        public long OwnerId { get; set; }
        
        public bool IsDefault { get; set; }
        public bool IsPublic { get; set; }
        public bool IsQuickAdd { get; set; }
        
        [MinLength(7)]
        public string? Color { get; set; }

        public Icon? Icon { get; set; }
        public long? IconId { get; set; }
        
        // Stories
        public ICollection<Story> Stories { get; set; }
    
    }
}