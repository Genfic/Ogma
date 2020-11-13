using System.Linq;
using Markdig;
using Ogma3.Data.DTOs;
using Ogma3.Data.Models;

namespace Ogma3.Data.Projections
{
    public static class CommentProjections
    {
        public static IQueryable<CommentDto> ToDto(this IQueryable<Comment> source, long? userId, MarkdownPipeline md)
        {
            return source.Select(c => new CommentDto
            {
                Id = c.Id,
                DateTime = c.DateTime,
                LastEdit = c.LastEdit,
                DeletedBy = c.DeletedBy,
                EditCount = c.EditCount ?? 0,
                Body = Markdown.ToHtml(c.Body, md),
                Owned = c.AuthorId == userId,
                IsBlocked = c.Author != null && c.Author.BlockedByUsers.Any(bu => bu.Id == userId),
                Author = c.Author == null
                    ? null
                    : new UserSimpleDto
                    {
                        UserName = c.Author.UserName,
                        Avatar = c.Author.Avatar,
                        Title = c.Author.Title,
                        Roles = c.Author.Roles.Select(r => new RoleDto
                        {
                            Id = r.Id,
                            Name = r.Name,
                            IsStaff = r.IsStaff,
                            Order = (int) r.Order,
                            Color = r.Color
                        })
                    }
            });
        }
    }
}