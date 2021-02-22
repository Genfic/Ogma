namespace Ogma3.Data.Models
{
    public class CommentsThreadSubscriber
    {
        public CommentsThread CommentsThread { get; set; }
        public long CommentsThreadId { get; set; }

        public OgmaUser OgmaUser { get; set; }
        public long OgmaUserId { get; set; }
    }
}