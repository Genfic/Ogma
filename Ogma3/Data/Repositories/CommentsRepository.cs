using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Markdig;
using MarkdigExtensions.Mentions;
using MarkdigExtensions.Spoiler;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data.DTOs;
using Ogma3.Data.Models;
using Ogma3.Data.Projections;
using Ogma3.Services.UserService;
using Utils.Extensions;

namespace Ogma3.Data.Repositories
{
    public class CommentsRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly long? _uid;
        private readonly MarkdownPipeline _md;

        public CommentsRepository(ApplicationDbContext context,IUserService userService)
        {
            _context = context;
            _uid = userService.GetUser()?.GetNumericId();
            _md = new MarkdownPipelineBuilder()
                .UseMentions(new MentionOptions("/user/", "_blank"))
                .UseAutoLinks()
                .UseAutoIdentifiers()
                .UseSpoilers()
                .Build();
        }


        public async Task<CommentDto> GetSingle(long id)
        {
            var comment =  await _context.Comments
                .Where(c => c.Id == id)                
                .ToDto(_uid, _md)
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
                .Where(c => c.CommentsThreadId == threadId)
                .OrderByDescending(c => c.DateTime)
                .ToDto(_uid, _md)
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