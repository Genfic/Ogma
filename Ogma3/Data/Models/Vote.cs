using System.ComponentModel.DataAnnotations;

namespace Ogma3.Data.Models
{
    public class Vote : BaseModel
    {
        [Required] 
        public User User { get; set; }
    }
}