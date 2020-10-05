using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data.DTOs;
using Ogma3.Data.Models;
using Ogma3.Pages.Shared;

namespace Ogma3.Data.Repositories
{
    public class CommentsRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public CommentsRepository(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }


        public async Task<CommentDto> GetSingle(long id)
        {
            return await _context.Comments
                .Where(c => c.Id == id)
                .ProjectTo<CommentDto>(_mapper.ConfigurationProvider)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<CommentDto>> GetPaginated(long threadId, int page, int perPage)
        {
            return await _context.Comments
                .Where(c => c.CommentsThreadId == threadId)
                .OrderByDescending(c => c.DateTime)
                .ProjectTo<CommentDto>(_mapper.ConfigurationProvider)
                .Paginate(page, perPage)
                .AsNoTracking()
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