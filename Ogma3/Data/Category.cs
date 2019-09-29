using System.ComponentModel.DataAnnotations;

namespace Ogma3.Data
{
    public class Category
    {
        [Key]
        [Required]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(20)]
        public string Name { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Description { get; set; }
    }
}