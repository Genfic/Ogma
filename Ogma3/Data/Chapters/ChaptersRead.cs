using System.Collections.Generic;
using Ogma3.Data.Bases;
using Ogma3.Data.Stories;
using Ogma3.Data.Users;

namespace Ogma3.Data.Chapters
{
    public class ChaptersRead : BaseModel
    {
        public Story Story { get; init; }
        public long StoryId { get; init; }
        public OgmaUser User { get; init; }
        public long UserId { get; init; }
        public List<long> Chapters { get; init; }
    }
}