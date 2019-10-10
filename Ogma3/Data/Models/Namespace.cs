using System.ComponentModel.DataAnnotations;

namespace Ogma3.Data.Models
{
    public class Namespace
    {
        [Key]
        [Required]
        public int Id { get; set; }
        
        [Required]
        [MinLength(CTConfig.Namespace.MinNameLength)]
        [MaxLength(CTConfig.Namespace.MaxNameLength)]
        public string Name { get; set; }
    }
}