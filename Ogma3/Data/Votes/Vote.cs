using Ogma3.Data.Bases;
using Ogma3.Data.Users;

namespace Ogma3.Data.Votes
{
    public class Vote : BaseModel
    {
        public OgmaUser User { get; set; }
        public long UserId { get; set; }
        public long StoryId { get; set; }
    }
}