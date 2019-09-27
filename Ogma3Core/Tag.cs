using System.ComponentModel.DataAnnotations;

namespace Ogma3Core
{
    public class Tag
    {
        [Key]
        [Required]
        public uint Id { get; set; }
        
        [Required]
        public string Name { get; set; }
        
        [Required]
        public string Description { get; set; }
    }
}