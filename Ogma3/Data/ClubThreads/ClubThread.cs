#nullable disable

using Ogma3.Data.Bases;
using Ogma3.Data.Clubs;
using Ogma3.Data.CommentsThreads;
using Ogma3.Data.Users;

namespace Ogma3.Data.ClubThreads;

public sealed class ClubThread : BaseModel
{
	public string Title { get; set; }
	public string Body { get; set; }
	public OgmaUser Author { get; init; }
	public long AuthorId { get; init; }
	public DateTimeOffset CreationDate { get; init; }
	public CommentsThread CommentsThread { get; init; }
	public Club Club { get; init; }
	public long ClubId { get; init; }
	public DateTimeOffset? DeletedAt { get; init; }
	public bool IsPinned { get; set; }
}