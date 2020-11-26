using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data.DTOs;
using Ogma3.Services.UserService;
using Utils.Extensions;

namespace Ogma3.Data.Repositories
{
    public class CommentsRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly long? _uid;
        private readonly IMapper _mapper;

        public CommentsRepository(ApplicationDbContext context,IUserService userService, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _uid = userService.GetUser()?.GetNumericId();
        }


        public async Task<CommentDto> GetSingle(long id)
        {
            var comment =  await _context.Comments
                .Where(c => c.Id == id)       
                .ProjectTo<CommentDto>(_mapper.ConfigurationProvider, new { currentUser = _uid })
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
                .ProjectTo<CommentDto>(_mapper.ConfigurationProvider, new { currentUser = _uid })
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