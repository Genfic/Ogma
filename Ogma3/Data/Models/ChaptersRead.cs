using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ogma3.Data.Models
{
    public class ChaptersRead : BaseModel
    {
        [Required]
        public Story Story { get; set; }
        public long StoryId { get; set; }
        
        [Required]
        public OgmaUser User { get; set; }
        public long UserId { get; set; }
        
        public List<long> Chapters { get; set; }
    }
}