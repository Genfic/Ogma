using System.ComponentModel.DataAnnotations;

namespace Ogma3.Data.Models
{
    public class Vote : BaseModel
    {
        [Required] 
        public User User { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public int VotePoolId { get; set; }
    }
}