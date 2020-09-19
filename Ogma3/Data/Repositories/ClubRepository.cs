using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data.DTOs;
using Ogma3.Pages.Shared;

namespace Ogma3.Data.Repositories
{
    public class ClubRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ClubRepository(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ClubBar> GetClubBar(long clubId, long? userId)
        {
            return await _context.Clubs
                .Where(c => c.Id == clubId)
                .ProjectTo<ClubBar>(_mapper.ConfigurationProvider, new { currentUser = userId })
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }
    }
}