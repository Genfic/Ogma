#nullable disable

using AutoDbSetGenerators;
using Ogma3.Data.Users;

namespace Ogma3.Data.CommentsThreads;

[AutoDbSet]
public sealed class CommentThreadSubscriber
{
	public CommentThread CommentThread { get; init; }
	public long CommentsThreadId { get; init; }
	public OgmaUser OgmaUser { get; init; }
	public long OgmaUserId { get; init; }
}