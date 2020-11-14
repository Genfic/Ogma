using System;
using System.Linq;
using Markdig;
using Ogma3.Data.DTOs;
using Ogma3.Data.Models;

namespace Ogma3.Data.Projections
{
    public static class CommentProjections
    {
        public static IQueryable<CommentDto> ToDto(this IQueryable<Comment> source, long? userId, MarkdownPipeline md) 
            => source.Select(c => new CommentDto
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
                            Order = r.Order ?? 0,
                            Color = r.Color
                        })
                    }
            });

        public static CommentDto ToDto(this Comment source, long? userId, MarkdownPipeline md) 
            => new CommentDto
            {
                Id = source.Id,
                DateTime = source.DateTime,
                LastEdit = source.LastEdit,
                DeletedBy = source.DeletedBy,
                EditCount = source.EditCount ?? 0,
                Body = Markdown.ToHtml(source.Body, md),
                Owned = source.AuthorId == userId,
                IsBlocked = source.Author != null && source.Author.BlockedByUsers.Any(bu => bu.Id == userId),
                Author = source.Author == null
                    ? null
                    : new UserSimpleDto
                    {
                        UserName = source.Author.UserName,
                        Avatar = source.Author.Avatar,
                        Title = source.Author.Title,
                        Roles = source.Author.Roles.Select(r => new RoleDto
                        {
                            Id = r.Id,
                            Name = r.Name,
                            IsStaff = r.IsStaff,
                            Order = r.Order ?? 0,
                            Color = r.Color
                        })
                    }
            };

        public static IQueryable<CommentRevisionDto> ToCommentRevisionDto(this IQueryable<CommentRevision> source, MarkdownPipeline md)
            => source.Select(c => new CommentRevisionDto
            {
                Body = Markdown.ToHtml(c.Body, md),
                EditTime = c.EditTime
            });
    }
}