using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Ogma3.Pages.Shared;

namespace Ogma3.Data.Repositories
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

        public async Task<ChapterDetails> GetChapterDetails(long id)
        {
            return await _context.Chapters
                .TagWith($"{nameof(ChaptersRepository)}.{nameof(GetChapterDetails)} -> {id}")
                .ProjectTo<ChapterDetails>(_mapper.ConfigurationProvider)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);
        }
    }
}