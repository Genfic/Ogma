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
                .Where(b => b.IsPublished)
                .Where(b => b.ContentBlockId == null)
                .Blacklist(_context, _uid)
                .SortByEnum(sort)
                .Take(count)
                .ProjectTo<StoryCard>(_mapper.ConfigurationProvider)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<StoryCard>> GetPaginatedCardsOfFolder(long folderId, int page, int perPage)
        {
            return await _context.FolderStories
                .TagWith($"{nameof(StoriesRepository)}.{nameof(GetPaginatedCardsOfFolder)} -> {folderId}")
                .Where(s => s.FolderId == folderId)
                .Select(s => s.Story)
                .Where(b => b.IsPublished)
                .Where(b => b.ContentBlockId == null)
                .Blacklist(_context, _uid)
                .OrderByDescending(s => s.ReleaseDate)
                .Paginate(page, perPage)
                .ProjectTo<StoryCard>(_mapper.ConfigurationProvider)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}