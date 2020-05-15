using System.Collections.Generic;

namespace Ogma3.Data.Models
{
    public class VotePool : BaseModel
    {
        public  ICollection<Vote> Votes { get; set; } = new List<Vote>();

        public Story? Story { get; set; }
        public long? StoryId { get; set; }
        public Blogpost? Blogpost { get; set; }
        public long? BlogpostId { get; set; }
    }
}