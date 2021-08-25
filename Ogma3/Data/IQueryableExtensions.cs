#nullable enable

using System;
using System.Linq;
using Ogma3.Data.Stories;

namespace Ogma3.Data
{
    public static class QueryableExtensions
    {
        /// <summary>
        /// Shorthand for `.Skip().Take()`
        /// </summary>
        /// <param name="query">`IQueryable` object</param>
        /// <param name="page">Page number</param>
        /// <param name="perPage">Number of results per page</param>
        /// <typeparam name="T">Type of the object</typeparam>
        /// <exception cref="ArgumentOutOfRangeException">Throw error where either `page` or `perPage` are less than 1</exception>
        /// <returns>Desired page of the objects</returns>
        public static IQueryable<T> Paginate<T>(this IQueryable<T> query, int page, int perPage)
        {
            if (page < 1) 
                throw new ArgumentOutOfRangeException(nameof(page), "Page has to be greater than 0");
            if (perPage < 1)
                throw new ArgumentOutOfRangeException(nameof(perPage), "PerPage has to be greater than 0");
            
            return query
                .Skip(Math.Max(0, page - 1) * perPage)
                .Take(perPage);
        }
        
        public static IQueryable<Story> SortByEnum(this IQueryable<Story> query, EStorySortingOptions order)
        {
            return order switch
            {
                EStorySortingOptions.TitleAscending => query.OrderBy(s => s.Title),
                EStorySortingOptions.TitleDescending => query.OrderByDescending(s => s.Title),
                EStorySortingOptions.DateAscending => query.OrderBy(s => s.CreationDate),
                EStorySortingOptions.DateDescending => query.OrderByDescending(s => s.CreationDate),
                EStorySortingOptions.WordsAscending => query.OrderBy(s => s.WordCount),
                EStorySortingOptions.WordsDescending => query.OrderByDescending(s => s.WordCount),
                EStorySortingOptions.ScoreAscending => query.OrderBy(s => s.Votes.Count),
                EStorySortingOptions.ScoreDescending => query.OrderByDescending(s => s.Votes.Count),
                EStorySortingOptions.UpdatedAscending => query.OrderBy(s => s.Chapters.OrderBy(c => c.PublicationDate).First().PublicationDate),
                EStorySortingOptions.UpdatedDescending => query.OrderByDescending(s => s.Chapters.OrderBy(c => c.PublicationDate).First().PublicationDate),
                _ => query.OrderByDescending(s => s.CreationDate)
            };
        }

        public static IQueryable<Story> Blacklist(this IQueryable<Story> query, ApplicationDbContext ctx, long? userId)
        {
            return userId is not null
                ? query
                    .Where(s => !ctx.BlacklistedRatings
                        .Where(br => br.UserId == userId)
                        .Select(br => br.Rating)
                        .Contains(s.Rating)
                    )
                    .Where(s => !ctx.BlacklistedTags
                        .Where(bt => bt.UserId == userId)
                        .Select(bt => bt.TagId)
                        .Intersect(s.Tags.Select(st => st.Id))
                        .Any()
                    )
                    .Where(s => !ctx.BlacklistedUsers
                        .Where(bu => bu.BlockingUserId == userId)
                        .Select(bu => bu.BlockedUserId)
                        .Contains(s.AuthorId)
                    )
                : query
                    .Where(s => !s.Rating.BlacklistedByDefault);
        }
    }
}