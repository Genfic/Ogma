using System.ComponentModel.DataAnnotations;
using Ogma3.Data.Bases;

namespace Ogma3.Data.Icons
{
    public class Icon : BaseModel
    {
        [Required]
        [MaxLength(32)]
        public string Name { get; set; }
    }
}