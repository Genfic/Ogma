using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Castle.Core.Internal;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data.Enums;
using Ogma3.Pages.Shared;
using Ogma3.Data.Models;

namespace Ogma3.Data.Repositories
{
    public class StoriesRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public StoriesRepository(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Get `StoryDetails` viewmodel by story OD
        /// </summary>
        /// <param name="id">ID of the desired story</param>
        /// <returns>Desired `StoryDetails` object</returns>
        public async Task<StoryDetails> GetStoryDetails(long id)
        {
            return await _context.Stories
                .TagWith($"{nameof(StoriesRepository)}.{nameof(GetStoryDetails)} -> {id}")
                .Where(s => s.Id == id)
                .ProjectTo<StoryDetails>(_mapper.ConfigurationProvider)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Get `StoryCard` objects, sorted according to `EStorySortingOptions` and paginated
        /// </summary>
        /// <param name="perPage">Number of objects per page</param>
        /// <param name="page">Number of the desired page</param>
        /// <param name="sort">Sorting method</param>
        /// <returns>Sorted and paginated list of `StoryCard` objects</returns>
        public async Task<List<StoryCard>> GetAndSortPaginatedStoryCards(int perPage = 10, int page = 1, EStorySortingOptions sort = EStorySortingOptions.DateDescending)
        {
            return await _context.Stories
                .TagWith($"{nameof(StoriesRepository)}.{nameof(GetAndSortPaginatedStoryCards)} -> {perPage}, {page}, {sort}")
                .SortByEnum(sort)
                .Paginate(page, perPage)
                .ProjectTo<StoryCard>(_mapper.ConfigurationProvider)
                .AsNoTracking()
                .ToListAsync();
        }

        /// <summary>
        /// Get `StoryCard` objects, sorted according to `EStorySortingOptions`, filtered, and paginated
        /// </summary>
        /// <param name="perPage">Number of objects per page</param>
        /// <param name="page">Number of the desired page</param>
        /// <param name="tags">Tags to search by</param>
        /// <param name="searchQuery">Query to search the titles by</param>
        /// <param name="ratingId">Rating to filter by</param>
        /// <param name="sort">Sorting method</param>
        /// <returns>Sorted, filtered, and paginated list of `StoryCard` objects</returns>
        public async Task<List<StoryCard>> SearchAndSortStoryCards(
            int perPage = 10,
            int page = 1,
            IList<long>? tags = null,
            string? searchQuery = null, 
            long? ratingId = null,
            EStorySortingOptions sort = EStorySortingOptions.DateDescending
        )
        {
            return await Search(tags, searchQuery, ratingId)
                .SortByEnum(sort)
                .ProjectTo<StoryCard>(_mapper.ConfigurationProvider)
                .Paginate(page, perPage)
                .AsNoTracking()
                .ToListAsync();
        }

        /// <summary>
        /// Count `Story` search results
        /// </summary>
        /// <param name="tags">Tags to search by</param>
        /// <param name="searchQuery">Query to search the titles by</param>
        /// <param name="ratingId">Rating to filter by</param>
        /// <returns>Number of stories that fit the requirements</returns>
        public async Task<int> CountSearchResults(IList<long>? tags = null, string? searchQuery = null, long? ratingId = null)
        {
            return await Search(tags, searchQuery, ratingId)
                .CountAsync();
        }

        /// <summary>
        /// Count the number of stories written by a user
        /// </summary>
        /// <param name="id">ID of the user</param>
        /// <returns>Number of stories written by the user</returns>
        public async Task<int> CountForUser(long id)
        {
            return await _context.Stories
                .TagWith($"{nameof(StoriesRepository)}.{nameof(CountForUser)} -> {id}")
                .Where(s => s.Author.Id == id)
                .CountAsync();
        }

        
        /// <summary>
        /// Apply a filter on `IQueryable` 
        /// </summary>
        /// <param name="tags">Tags to search by</param>
        /// <param name="searchQuery">Query to search the titles by</param>
        /// <param name="ratingId">Rating to filter by</param>
        /// <returns>`IQueryable` objects with applied filters</returns>
        private IQueryable<Story> Search(IList<long>? tags = null, string? searchQuery = null, long? ratingId = null)
        {
            // Prepare search query
            var query = _context.Stories
                .AsQueryable();
            
            // Search by title
            if (!searchQuery.IsNullOrEmpty())
                query = query.Where(s => EF.Functions.Like(s.Title.ToUpper(), $"%{searchQuery.Trim().ToUpper()}%"));
            
            // Search by rating
            if (ratingId != null)
                query = query.Where(s => s.Rating.Id == ratingId);
            
            // Search by tags
            if (tags != null && tags.Count > 0)
                query = query.Where(s => s.StoryTags.Any(st => tags.Contains(st.TagId)));

            return query;
        }
    }
}