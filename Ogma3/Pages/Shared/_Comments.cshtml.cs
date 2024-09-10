using Ogma3.Data.Comments;

namespace Ogma3.Pages.Shared;

public record CommentsThreadDto(long Id, CommentSource Type, DateTime? LockDate);