using Ogma3.Data.Users;

namespace Ogma3.Data.CommentsThreads
{
    public class CommentsThreadSubscriber
    {
        public CommentsThread CommentsThread { get; set; }
        public long CommentsThreadId { get; set; }

        public OgmaUser OgmaUser { get; set; }
        public long OgmaUserId { get; set; }
    }
}