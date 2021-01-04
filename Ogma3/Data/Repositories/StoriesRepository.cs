using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data.Enums;
using Ogma3.Data.Models;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Pages.Shared.Cards;
using Ogma3.Pages.Shared.Details;
using Ogma3.Pages.Shared.Minimals;

namespace Ogma3.Data.Repositories
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
        /// Get `StoryDetails` viewmodel by story OD
        /// </summary>
        /// <param name="id">ID of the desired story</param>
        /// <returns>Desired `StoryDetails` object</returns>
        public async Task<StoryDetails> GetStoryDetails(long id)
        {
            return await _context.Stories
                .TagWith($"{nameof(StoriesRepository)}.{nameof(GetStoryDetails)} -> {id}")
                .Where(s => s.Id == id)
                .ProjectTo<StoryDetails>(_mapper.ConfigurationProvider, new{ currentUser = _uid })
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        public async Task<StoryCard> GetCard(long storyId)
        {
            return await _context.Stories
                .Where(s => s.Id == storyId)
                .ProjectTo<StoryCard>(_mapper.ConfigurationProvider)
                .AsNoTracking()
                .FirstOrDefaultAsync();
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
        /// Get published `StoryCard` objects, sorted according to `EStorySortingOptions`
        /// and paginated, with a blacklist applied
        /// </summary>
        /// <param name="perPage">Number of objects per page</param>
        /// <param name="page">Number of the desired page</param>
        /// <param name="authorId">ID of the author whose stories are to be fetched</param>
        /// <param name="sort">Sorting method</param>
        /// <returns>Sorted and paginated list of `StoryCard` objects</returns>
        public async Task<List<StoryCard>> GetAndSortPaginatedStoryCards(
            int perPage, 
            int page, 
            long authorId,
            EStorySortingOptions sort = EStorySortingOptions.DateDescending
        ) {
            return await _context.Stories
                .TagWith($"{nameof(StoriesRepository)}.{nameof(GetAndSortPaginatedStoryCards)} -> {perPage}, {page}, {authorId}, {sort}")
                .Where(b => b.IsPublished)
                .Where(b => b.ContentBlockId == null)
                .Where(b => b.AuthorId == authorId)
                .Blacklist(_context, _uid)
                .SortByEnum(sort)
                .Paginate(page, perPage)
                .ProjectTo<StoryCard>(_mapper.ConfigurationProvider)
                .AsNoTracking()
                .ToListAsync();
        }

        /// <summary>
        /// Get `StoryCard` objects, sorted according to `EStorySortingOptions` and paginated
        /// </summary>
        /// <param name="perPage">Number of objects per page</param>
        /// <param name="page">Number of the desired page</param>
        /// <param name="sort">Sorting method</param>
        /// <returns>Sorted and paginated list of `StoryCard` objects</returns>
        public async Task<List<StoryCard>> GetAndSortOwnedPaginatedStoryCards(int perPage, int page, long authorId, EStorySortingOptions sort = EStorySortingOptions.DateDescending)
        {
            return await _context.Stories
                .TagWith($"{nameof(StoriesRepository)}.{nameof(GetAndSortOwnedPaginatedStoryCards)} -> {perPage}, {page}, {sort}")
                .Where(b => b.AuthorId == authorId)
                .SortByEnum(sort)
                .Paginate(page, perPage)
                .ProjectTo<StoryCard>(_mapper.ConfigurationProvider)
                .AsNoTracking()
                .ToListAsync();
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
            int perPage,
            int page,
            IList<long>? tags = null,
            string? searchQuery = null, 
            long? ratingId = null,
            EStorySortingOptions sort = EStorySortingOptions.DateDescending
        )
        {
            return await Search(tags, searchQuery, ratingId)
                .TagWith($"{nameof(StoriesRepository)}.{nameof(SearchAndSortStoryCards)} -> {perPage}, {page}, {searchQuery}. {ratingId}, {sort}")
                .Where(b => b.IsPublished)
                .Where(b => b.ContentBlockId == null)
                .Blacklist(_context, _uid)
                .SortByEnum(sort)
                .Paginate(page, perPage)
                .ProjectTo<StoryCard>(_mapper.ConfigurationProvider)
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
        public async Task<int> CountSearchResults(
            IList<long>? tags = null, 
            string? searchQuery = null, 
            long? ratingId = null
        )
        {
            return await Search(tags, searchQuery, ratingId)
                .TagWith($"{nameof(StoriesRepository)}.{nameof(CountSearchResults)} -> {searchQuery}. {ratingId}")
                .Where(b => b.IsPublished)
                .Where(b => b.ContentBlockId == null)
                .Blacklist(_context, _uid)
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
                .Where(b => b.IsPublished)
                .Where(b => b.ContentBlockId == null)
                .Where(s => s.AuthorId == id)
                .CountAsync();
        }

        /// <summary>
        /// Count the number of stories written by a user
        /// </summary>
        /// <param name="id">ID of the user</param>
        /// <returns>Number of stories written by the user</returns>
        public async Task<int> CountOwnedForUser(long id)
        {
            return await _context.Stories
                .TagWith($"{nameof(StoriesRepository)}.{nameof(CountForUser)} -> {id}")
                .Where(s => s.Author.Id == id)
                .CountAsync();
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
        
        
        /// <summary>
        /// Apply a filter on `IQueryable` 
        /// </summary>
        /// <param name="tags">Tags to search by</param>
        /// <param name="searchQuery">Query to search the titles by</param>
        /// <param name="ratingId">Rating to filter by</param>
        /// <returns>`IQueryable` objects with applied filters</returns>
        private IQueryable<Story> Search(ICollection<long> tags = null, string? searchQuery = null, long? ratingId = null)
        {
            // Prepare search query
            var query = _context.Stories
                .AsQueryable();
            
            // Search by title
            if (!string.IsNullOrEmpty(searchQuery))
                query = query.Where(s => EF.Functions.Like(s.Title.ToUpper(), $"%{searchQuery.Trim().ToUpper()}%"));
            
            // Search by rating
            if (ratingId != null)
                query = query.Where(s => s.Rating.Id == ratingId);
            
            // Search by tags
            if (tags != null && tags.Count > 0)
                query = query.Where(s => s.Tags.Any(st => tags.Contains(st.Id)));

            return query;
        }
    }
}