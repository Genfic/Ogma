using System.ComponentModel.DataAnnotations;

namespace Ogma3.Data.Models
{
    public class Vote : BaseModel
    {
        [Required] 
        public OgmaUser User { get; set; }

        [Required]
        public long UserId { get; set; }

        [Required]
        public long StoryId { get; set; }
    }
}