using System.ComponentModel.DataAnnotations;

namespace Ogma3.Data.Models
{
    public class Quote : BaseModel
    {
        [Required]
        [MinLength(3)]
        public string Body { get; set; }
        
        [Required]
        [MinLength(3)]
        public string Author { get; set; }
    }
}