using AutoDbSetGenerators;
using Ogma3.Data.Bases;
using Ogma3.Data.Clubs;
using Ogma3.Data.CommentsThreads;
using Ogma3.Data.Users;

namespace Ogma3.Data.ClubThreads;

[AutoDbSet]
public sealed class ClubThread : BaseModel
{
	public required string Title { get; set; }
	public required string Body { get; set; }
	public OgmaUser Author { get; init; } = null!;
	public long AuthorId { get; init; }
	public DateTimeOffset CreationDate { get; init; }
	public CommentThread CommentThread { get; init; } = null!;
	public Club Club { get; init; } = null!;
	public long ClubId { get; init; }
	public DateTimeOffset? DeletedAt { get; init; }
	public bool IsPinned { get; set; }
}