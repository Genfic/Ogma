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
        

        public async Task<IEnumerable<CommentDto>> GetPaginated(long threadId, int page, int perPage)
        {
            return await _context.Comments
                .Where(c => c.CommentsThreadId == threadId)
                .OrderBy(c => c.DateTime)
                .Paginate(page, perPage)
                .ProjectTo<CommentDto>(_mapper.ConfigurationProvider)
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

        public async Task<ICollection<BlogpostCard>> GetPaginatedCardsForUser(string userName, int page, int perPage, bool publishedOnly = true)
        {
            return await _context.Blogposts
                .Where(b => b.Author.NormalizedUserName == userName.Normalize().ToUpper())
                .Where(b => b.IsPublished || !publishedOnly)
                .Paginate(page, perPage)
                .ProjectTo<BlogpostCard>(_mapper.ConfigurationProvider)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<int> CountForUser(string userName, bool publishedOnly = true)
        {
            return await _context.Blogposts
                .Where(b => b.Author.NormalizedUserName == userName.Normalize().ToUpper())
                .Where(b => b.IsPublished || !publishedOnly)
                .CountAsync();
        }

        public async Task<BlogpostDetails> GetDetails(long id)
        {
            return await _context.Blogposts
                .Where(b => b.Id == id)
                .ProjectTo<BlogpostDetails>(_mapper.ConfigurationProvider)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }
    }
}