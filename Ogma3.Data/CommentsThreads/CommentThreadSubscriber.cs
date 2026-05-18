using AutoDbSetGenerators;
using Ogma3.Data.Users;

namespace Ogma3.Data.CommentsThreads;

[AutoDbSet]
public sealed class CommentThreadSubscriber
{
	public CommentThread CommentThread { get; init; } = null!;
	public long CommentsThreadId { get; init; }
	public OgmaUser OgmaUser { get; init; } = null!;
	public long OgmaUserId { get; init; }
}