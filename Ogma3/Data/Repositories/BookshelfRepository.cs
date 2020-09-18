using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Ogma3.Pages.Shared;

namespace Ogma3.Data.Repositories
{
    public class BookshelfRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public BookshelfRepository(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<BookshelfDetails> GetBookshelfDetails(long id)
        {
            return await _context.Shelves
                .Where(s => s.Id == id)
                .ProjectTo<BookshelfDetails>(_mapper.ConfigurationProvider)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }
    }
}