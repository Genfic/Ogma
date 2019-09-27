using System.ComponentModel.DataAnnotations;

namespace Ogma3.Data
{
    public class Tag
    {
        [Key]
        [Required]
        public int Id { get; set; }
        
        [Required]
        public string Name { get; set; }
        
        [Required]
        public string Description { get; set; }
    }
}