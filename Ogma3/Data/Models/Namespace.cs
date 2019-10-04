using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ogma3.Data.Models
{
    public class Namespace
    {
        [Key]
        [Required]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(20)]
        public string Name { get; set; }
        
        public ICollection<Tag> Tags { get; set; }
    }
}