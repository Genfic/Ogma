using System.ComponentModel.DataAnnotations;

namespace Ogma3.Data.Models
{
    public class Icon : BaseModel
    {
        [Required]
        [MaxLength(32)]
        public string Name { get; set; }
    }
}