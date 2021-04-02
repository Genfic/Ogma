using System.ComponentModel.DataAnnotations;
using Ogma3.Data.Bases;

namespace Ogma3.Data.Quotes
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