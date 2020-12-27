using System.Collections.Generic;

namespace Ogma3.Data.Models
{
    public class ChaptersRead : BaseModel
    {
        public Story Story { get; set; }
        public long StoryId { get; set; }
        public OgmaUser User { get; set; }
        public long UserId { get; set; }
        
        public List<long> Chapters { get; set; }
    }
}