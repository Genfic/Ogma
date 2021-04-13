using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Ogma3.Pages.Shared.Cards;
using Ogma3.Pages.Shared.Details;

namespace Ogma3.Data.Blogposts
{
    public class BlogpostsRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public BlogpostsRepository(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Get all blogposts written by the given user as blogpost cards
        /// </summary>
        /// <param name="userName">Name of the desired author</param>
        /// <param name="page">Page number</param>
        /// <param name="perPage">Amount of blogposts per page</param>
        /// <returns>Collection of blogpost cards</returns>
        public async Task<ICollection<BlogpostCard>> GetAllPaginatedCardsForUser(string userName, int page, int perPage)
        {
            return await _context.Blogposts
                .TagWith($"{nameof(BlogpostsRepository)}.{nameof(GetAllPaginatedCardsForUser)} -> {userName} {page} {perPage}")
                .Where(b => b.Author.NormalizedUserName == userName.Normalize().ToUpper())
                .OrderByDescending(b => b.PublishDate)
                .Paginate(page, perPage)
                .ProjectTo<BlogpostCard>(_mapper.ConfigurationProvider)
                .AsNoTracking()
                .ToListAsync();
        }

        /// <summary>
        /// Get all blogposts written by the given user that are published and aren't blocked, as blogpost cards
        /// </summary>
        /// <param name="userName">Name of the desired author</param>
        /// <param name="page">Page number</param>
        /// <param name="perPage">Amount of blogposts per page</param>
        /// <returns>Collection of blogpost cards</returns>
        public async Task<ICollection<BlogpostCard>> GetPublicPaginatedCardsForUser(string userName, int page, int perPage)
        {
            return await _context.Blogposts
                .TagWith($"{nameof(BlogpostsRepository)}.{nameof(GetPublicPaginatedCardsForUser)} -> {userName} {page} {perPage}")
                .Where(b => b.Author.NormalizedUserName == userName.Normalize().ToUpper())
                .Where(b => b.IsPublished)
                .Where(b => b.ContentBlockId == null)
                .OrderByDescending(b => b.PublishDate)
                .Paginate(page, perPage)
                .ProjectTo<BlogpostCard>(_mapper.ConfigurationProvider)
                .AsNoTracking()
                .ToListAsync();
        }

        /// <summary>
        /// Count all of user's blogposts
        /// </summary>
        /// <param name="userName">Name of the desired author</param>
        /// <returns>The amount of blogposts</returns>
        public async Task<int> CountAllForUser(string userName)
        {
            return await _context.Blogposts
                .Where(b => b.Author.NormalizedUserName == userName.Normalize().ToUpper())
                .CountAsync();
        }

        /// <summary>
        /// Count all of user's blogposts that are published and aren't blocked
        /// </summary>
        /// <param name="userName">Name of the desired author</param>
        /// <returns>The amount of blogposts</returns>
        public async Task<int> CountPublicForUser(string userName)
        {
            return await _context.Blogposts
                .Where(b => b.Author.NormalizedUserName == userName.Normalize().ToUpper())
                .Where(b => b.IsPublished)
                .Where(b => b.ContentBlockId == null)
                .CountAsync();
        }
    }
}