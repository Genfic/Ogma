using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data.Tags;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Pages.Shared.Cards;
using Ogma3.Pages.Shared.Details;
using Ogma3.Pages.Shared.Minimals;

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

        public async Task<StoryMinimal> GetMinimal(long id)
        {
            return await _context.Stories
                .Where(c => c.Id == id)
                .Where(c => c.IsPublished)
                .Where(b => b.ContentBlockId == null)
                .ProjectTo<StoryMinimal>(_mapper.ConfigurationProvider)
                .AsNoTracking()
                .FirstOrDefaultAsync();
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

        /// <summary>
        /// Gets `StoryCard` objects with a given Tag, paginates them, and sorts by release date descending
        /// </summary>
        /// <param name="tagId">Tag to search for</param>
        /// <param name="perPage">Number of objects per page</param>
        /// <param name="page">Number of the desired page</param>
        /// <returns>Sorted, filtered, and paginated list of `StoryCard` objects</returns>
        public async Task<List<StoryCard>> GetCardsWithTag(long tagId, int page, int perPage)
        {
            return await _context.Stories
                .TagWith($"{nameof(StoriesRepository)}.{nameof(GetCardsWithTag)} -> {tagId}")
                .Where(b => b.IsPublished)
                .Where(b => b.ContentBlockId == null)
                .Where(s => s.Tags.Any(st => st.Id == tagId))
                .Blacklist(_context, _uid)
                .OrderByDescending(s => s.ReleaseDate)
                .Paginate(page, perPage)
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

        /// <summary>
        /// Counts stories with the given tag
        /// </summary>
        /// <param name="tagId">Tag to search for</param>
        /// <returns>Number of stories</returns>
        public async Task<int> CountWithTag(long tagId)
        {
            return await _context.Stories
                .TagWith($"{nameof(StoriesRepository)}.{nameof(CountWithTag)} -> {tagId}")
                .Where(b => b.IsPublished)
                .Where(b => b.ContentBlockId == null)
                .Where(s => s.Tags.Any(st => st.Id == tagId))
                .CountAsync();
        }
    }
}