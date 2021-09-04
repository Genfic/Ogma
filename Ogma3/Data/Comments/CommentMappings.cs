using System;
using System.Linq;
using System.Linq.Expressions;
using Ogma3.Data.Roles;
using Ogma3.Data.Users;
using Utils;

namespace Ogma3.Data.Comments
{
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
            EditCount = c.EditCount,
            LastEdit = c.LastEdit,
            IsBlocked = c.Author.BlockedByUsers.Any(bu => bu.Id == uid),
            Author = c.DeletedBy != null ? null : new UserSimpleDto
                {
                    Avatar = c.Author.Avatar,
                    Title = c.Author.Title,
                    UserName = c.Author.UserName,
                    Roles = c.Author.Roles.AsQueryable().Select(RoleMappings.ToRoleDto)
                }
        };
    }
}