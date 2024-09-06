using Markdig;
using Ogma3.Data.Roles;
using Ogma3.Data.Users;
using Ogma3.Infrastructure.Constants;

namespace Ogma3.Data.Comments;

public class CommentDto
{
	public required long Id { get; init; }
	public required UserSimpleDto? Author { get; init; }
	public required DateTime DateTime { get; init; }
	public required DateTime? LastEdit { get; init; }
	public required ushort EditCount { get; init; }
	public required bool Owned { get; set; }
	public required string? Body { get; set; }
	public required EDeletedBy? DeletedBy { get; init; }
	public required bool IsBlocked { get; init; }

	public static CommentDto FromComment (Comment comment, long? currentUser) => new()
	{
		Id = comment.Id,
		DateTime = comment.DateTime,
		EditCount = comment.EditCount,
		Owned = comment.AuthorId == currentUser,
		IsBlocked = comment.Author.Blockers.Any(bu => bu.Id == currentUser),
		Author = comment.DeletedBy != null ? null : new UserSimpleDto
		{
			UserName = comment.Author.UserName,
			Avatar = comment.Author.Avatar,
			Title = comment.Author.Title,
			Roles = comment.Author.Roles.Select(r => new RoleDto
			{
				Id = r.Id,
				Name = r.Name,
				Color = r.Color,
				IsStaff = r.IsStaff,
				Order = r.Order,
			}),
		},
		DeletedBy = comment.DeletedBy,
		LastEdit = comment.LastEdit,
		Body = comment.DeletedBy != null ? null : Markdown.ToHtml(comment.Body, MarkdownPipelines.Comment),
	};
}