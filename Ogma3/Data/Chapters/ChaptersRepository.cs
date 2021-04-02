using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Ogma3.Pages.Shared.Details;
using Ogma3.Pages.Shared.Minimals;

namespace Ogma3.Data.Chapters
{
    public class ChaptersRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ChaptersRepository(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ChapterDetails> GetChapterDetails(long id, long? currentUser)
        {
            return await _context.Chapters
                .TagWith($"{nameof(ChaptersRepository)}.{nameof(GetChapterDetails)} -> {id}")
                .Where(c => c.Id == id)
                .Where(c => c.IsPublished || c.Story.AuthorId == currentUser)
                .Where(c => c.ContentBlockId == null || c.Story.AuthorId == currentUser)
                .ProjectTo<ChapterDetails>(_mapper.ConfigurationProvider)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        public async Task<List<ChapterMicroDto>> GetMicroSiblings(long storyId, uint ordinal)
        {
            return await _context.Chapters
                .TagWith($"{nameof(ChaptersRepository)}.{nameof(GetMicroSiblings)} -> {storyId} {ordinal}")
                .Where(c => c.StoryId == storyId)
                .Where(c => c.Order == ordinal - 1 || c.Order == ordinal + 1)
                .Select(c => new ChapterMicroDto
                {
                    Id = c.Id,
                    Title = c.Title,
                    Slug = c.Slug,
                    Order = c.Order
                })
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<ChapterMinimal> GetMinimal(long id) //, bool publishedOnly = true)
        {
            return await _context.Chapters
                .Where(c => c.Id == id)
                .Where(c => c.IsPublished) // || !publishedOnly)
                .Where(b => b.ContentBlockId == null) // || !publishedOnly)
                .ProjectTo<ChapterMinimal>(_mapper.ConfigurationProvider)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }
    }
}