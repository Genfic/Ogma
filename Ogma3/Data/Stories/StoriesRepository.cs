using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Pages.Shared.Cards;

namespace Ogma3.Data.Stories
{
    public class StoriesRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly long? _uid;
        
        public StoriesRepository(ApplicationDbContext context, IMapper mapper, IHttpContextAccessor contextAccessor)
        {
            _context = context;
            _mapper = mapper;
            _uid = contextAccessor?.HttpContext?.User.GetNumericId();
        }

        /// <summary>
        /// Get `StoryCard` objects, sorted according to `EStorySortingOptions` and paginated
        /// </summary>
        /// <param name="count">Number of objects</param>
        /// <param name="sort">Sorting method</param>
        /// <returns>Sorted and paginated list of `StoryCard` objects</returns>
        public async Task<List<StoryCard>> GetTopStoryCards(int count, EStorySortingOptions sort = EStorySortingOptions.DateDescending)
        {
            return await _context.Stories
                .TagWith($"{nameof(StoriesRepository)}.{nameof(GetTopStoryCards)} -> {count}, {sort}")
                .Where(b => b.PublicationDate != null)
                .Where(b => b.ContentBlockId == null)
                .Blacklist(_context, _uid)
                .SortByEnum(sort)
                .Take(count)
                .ProjectTo<StoryCard>(_mapper.ConfigurationProvider)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}