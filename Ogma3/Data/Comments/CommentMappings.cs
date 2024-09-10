using System.Linq.Expressions;
using Ogma3.Data.Roles;
using Ogma3.Data.Users;

namespace Ogma3.Data.Comments;

public static class CommentMappings
{
	public static Expression<Func<Comment, CommentDto>> ToCommentDto(long? uid) => c => new CommentDto
	{
		Id = c.Id,
		Body = c.DeletedBy == null
			? c.Body
			: string.Empty,
		Owned = c.AuthorId == uid && c.DeletedBy == null,
		DateTime = c.DateTime,
		DeletedBy = c.DeletedBy,
		IsEdited = c.Revisions.Any(),
		IsBlocked = c.Author.Blockers.Any(bu => bu.Id == uid),
		Author = c.DeletedBy != null
			? null
			: new UserSimpleDto
			{
				Avatar = c.Author.Avatar,
				Title = c.Author.Title,
				UserName = c.Author.UserName,
				Roles = c.Author.Roles.AsQueryable().Select(RoleMappings.ToRoleDto),
			},
	};
}