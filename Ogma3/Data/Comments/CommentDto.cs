using System.Text.Json.Serialization;
using Ogma3.Data.Users;

namespace Ogma3.Data.Comments;

public sealed class CommentDto
{
	public required string Id { get; set; }

	[JsonIgnore]
	public long InternalId { get; set; }
	public required UserSimpleDto? Author { get; init; }
	public required DateTimeOffset DateTime { get; init; }
	// public required bool Owned { get; set; }
	public required string? Body { get; set; }
	public required EDeletedBy? DeletedBy { get; init; }
	public required bool IsBlocked { get; init; }

	public required bool IsEdited { get; init; }
}