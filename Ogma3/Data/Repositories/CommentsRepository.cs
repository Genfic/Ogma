using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Markdig;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data.DTOs;
using Ogma3.Infrastructure;
using Ogma3.Services.UserService;
using Utils.Extensions;

namespace Ogma3.Data.Repositories
{
    public class CommentsRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly long? _uid;
        private readonly IMapper _mapper;
        private readonly MarkdownPipeline _markdownPipeline;

        public CommentsRepository(ApplicationDbContext context,IUserService userService, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _uid = userService.GetUser()?.GetNumericId();
            _markdownPipeline = MarkdownPipelines.Comment;
        }


        public async Task<CommentDto> GetSingle(long id)
        {
            var comment =  await _context.Comments
                .Where(c => c.Id == id)       
                .ProjectTo<CommentDto>(_mapper.ConfigurationProvider, new { currentUser = _uid })
                .AsNoTracking()
                .FirstOrDefaultAsync();
            
            return comment;
        }
        
        public async Task<string> GetMarkdown(long id)
        {
            return await _context.Comments
                .Where(c => c.Id == id)
                .Select(c => c.Body)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<CommentDto>> GetPaginated(long threadId, int page, int perPage)
        {
            return await _context.Comments
                .TagWith($"{nameof(CommentsRepository)} -> {nameof(GetPaginated)} : {threadId} {page} {perPage}")
                .Where(c => c.CommentsThreadId == threadId)
                .OrderByDescending(c => c.DateTime)
                // .ProjectTo<CommentDto>(_mapper.ConfigurationProvider, new { currentUser = _uid })
                .Select(c => new CommentDto
                {
                    Id = c.Id,
                    Body = c.DeletedBy == null ? Markdown.ToHtml(c.Body, _markdownPipeline) : string.Empty,
                    Owned = c.AuthorId == _uid && c.DeletedBy == null,
                    DateTime = c.DateTime,
                    DeletedBy = c.DeletedBy,
                    EditCount = c.EditCount ?? 0,
                    LastEdit = c.LastEdit,
                    IsBlocked = c.Author.BlockedByUsers.Any(bu => bu.Id == _uid),
                    Author = c.DeletedBy != null ? null : new UserSimpleDto
                    {
                        Avatar = c.Author.Avatar,
                        Title = c.Author.Title,
                        UserName = c.Author.UserName,
                        Roles = c.Author.Roles.Select(r => new RoleDto
                        {
                            Id = r.Id,
                            Name = r.Name,
                            Order = (int) (r.Order ?? 0),
                            IsStaff = r.IsStaff,
                            Color = r.Color
                        })
                    }
                })
                .AsNoTracking()
                .Paginate(page, perPage)
                .ToListAsync();
            
        }

        public async Task<int> CountComments(long threadId)
        {
            return await _context.CommentThreads
                .Where(ct => ct.Id == threadId)
                .Select(ct => ct.CommentsCount)
                .FirstOrDefaultAsync();
        }


    }
}