using Ogma3.Data.Users;

namespace Ogma3.Data.CommentsThreads;

public class CommentsThreadSubscriber
{
	public CommentsThread CommentsThread { get; init; }
	public long CommentsThreadId { get; init; }
	public OgmaUser OgmaUser { get; init; }
	public long OgmaUserId { get; init; }
}