using Ogma3.Data.Users;
using Ogma3.Infrastructure.Sqids;

namespace Ogma3.Data.Comments;

public sealed class CommentDto
{
	public required Sqid Sqid { get; set; }
	public required UserSimpleDto? Author { get; init; }
	public required DateTimeOffset DateTime { get; init; }
	public required string? Body { get; set; }
	public required EDeletedBy? DeletedBy { get; init; }
	public required bool IsBlocked { get; init; }
	public required bool IsEdited { get; init; }
}