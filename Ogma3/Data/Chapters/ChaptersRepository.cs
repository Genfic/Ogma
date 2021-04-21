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