using Ogma3.Data.Comments;

namespace Ogma3.Pages.Shared;

public sealed class CommentsThreadDto
{
	public required long Id { get; init; }
	public required CommentSource Type { get; init; }
	public required DateTimeOffset? LockDate { get; init; }

}