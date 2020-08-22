using System.Collections.Generic;

namespace Ogma3.Data.Models
{
    public class CommentsThread : BaseModel
    {
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();

        public int CommentsCount { get; set; }
        
        public OgmaUser? User { get; set; }
        public long? UserId { get; set; }
        public Chapter? Chapter { get; set; }
        public long? ChapterId { get; set; }
        public Blogpost? Blogpost { get; set; }
        public long? BlogpostId { get; set; }
    }
}