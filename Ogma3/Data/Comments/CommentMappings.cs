using System.Linq.Expressions;
using Ogma3.Data.Roles;
using Ogma3.Data.Users;

namespace Ogma3.Data.Comments;

public static class CommentMappings
{
	public static Expression<Func<Comment, CommentDto>> ToCommentDto(long? uid) => c => new CommentDto
	{
		Id = string.Empty,
		InternalId = c.Id,
		Body = c.Body,
		DateTime = c.DateTime,
		DeletedBy = c.DeletedBy,
		IsEdited = c.Revisions.Any(),
		IsBlocked = c.Author.Blockers.Any(bu => bu.Id == uid),
		Author = c.DeletedBy != null
			? null
			: new UserSimpleDto
			{
				Avatar = c.Author.Avatar.Url,
				Title = c.Author.Title,
				UserName = c.Author.UserName,
				Roles = c.Author.Roles.Select(r => new RoleTinyDto(r.Name, r.Color, r.Order)),
			},
	};
}