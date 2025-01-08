using Ogma3.Data.Users;

namespace Ogma3.Data.Comments;

public sealed class CommentDto
{
	public required long Id { get; init; }
	public required UserSimpleDto? Author { get; init; }
	public required DateTimeOffset DateTime { get; init; }
	// public required bool Owned { get; set; }
	public required string? Body { get; set; }
	public required EDeletedBy? DeletedBy { get; init; }
	public required bool IsBlocked { get; init; }
	
	public required bool IsEdited { get; init; }
}