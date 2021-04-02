using System.Collections.Generic;
using Ogma3.Data.Bases;
using Ogma3.Data.Stories;
using Ogma3.Data.Users;

namespace Ogma3.Data.Chapters
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