using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Markdig;
using Microsoft.EntityFrameworkCore;
using Ogma3.Infrastructure.Constants;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Services.UserService;

namespace Ogma3.Data.Comments
{
    public class CommentsRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly long? _uid;
        private readonly MarkdownPipeline _markdownPipeline;

        public CommentsRepository(ApplicationDbContext context,IUserService userService)
        {
            _context = context;
            _uid = userService.GetUser()?.GetNumericId();
            _markdownPipeline = MarkdownPipelines.Comment;
        }


        public async Task<CommentDto> GetSingle(long id)
        {
            var comment =  await _context.Comments
                .Where(c => c.Id == id)       
                .Select(CommentMappings.ToCommentDto(_uid))
                .AsNoTracking()
                .FirstOrDefaultAsync();

            comment.Body = comment.Body is null 
                ? null 
                : Markdown.ToHtml(comment.Body, _markdownPipeline);
            
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
            var comments =  await _context.Comments
                .TagWith($"{nameof(CommentsRepository)} -> {nameof(GetPaginated)} : {threadId} {page} {perPage}")
                .Where(c => c.CommentsThreadId == threadId)
                .OrderByDescending(c => c.DateTime)
                .Select(CommentMappings.ToCommentDto(_uid))
                .AsNoTracking()
                .Paginate(page, perPage)
                .ToListAsync();
            
            comments.ForEach(c => c.Body = c.Body is null ? null : Markdown.ToHtml(c.Body, _markdownPipeline));
            return comments;
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