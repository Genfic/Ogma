using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Ogma3.Pages.Shared;
using Ogma3.Pages.Shared.Cards;
using Ogma3.Pages.Shared.Details;

namespace Ogma3.Data.Repositories
{
    public class ThreadRepository
    {
        private readonly ApplicationDbContext _context;

        public ThreadRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get `count` number of `ThreadCard` objects for a `Thread` of given `threadId`
        /// </summary>
        /// <param name="clubId">ID of the club</param>
        /// <param name="count">Desired number of objects</param>
        /// <returns>Returns a List of `ThreadCard` objects</returns>
        /// <exception cref="ArgumentOutOfRangeException">Occurs when card count is less than 1</exception>
        public async Task<List<ThreadCard>> GetThreadCards(long clubId, int count)
        {
            if (count < 1) 
                throw new ArgumentOutOfRangeException(nameof(count), "Count has to be greater than 0");
            
            return await _context.ClubThreads
                .Where(ct => ct.ClubId == clubId)
                .OrderByDescending(ct => ct.CreationDate)
                .Take(count)
                .Select(ct => new ThreadCard
                {
                    Id = ct.Id,
                    ClubId = ct.ClubId,
                    Title = ct.Title,
                    CreationDate = ct.CreationDate,
                    AuthorName = ct.Author.UserName,
                    AuthorAvatar = ct.Author.Avatar,
                    CommentsCount = ct.CommentsThread.Comments.Count
                })
                .AsNoTracking()
                .ToListAsync();
        }

        /// <summary>
        /// Get a specific page of `ThreadCard` objects of a `Club` with `clubId`
        /// </summary>
        /// <param name="clubId">ID of the club</param>
        /// <param name="page">Number of the page</param>
        /// <param name="perPage">Element count per page</param>
        /// <returns>Returns a List of `ThreadCard` objects</returns>
        /// <exception cref="ArgumentOutOfRangeException">Occurs when `page` or `perPage` is less than 1</exception>
        public async Task<List<ThreadCard>> GetThreadCards(long clubId, int page, int perPage)
        {
            if (page < 1) 
                throw new ArgumentOutOfRangeException(nameof(page), "Page has to be greater than 0");
            if (perPage < 1)
                throw new ArgumentOutOfRangeException(nameof(perPage), "PerPage has to be greater than 0");
            
            return await _context.ClubThreads
                .Where(ct => ct.ClubId == clubId)
                .OrderByDescending(ct => ct.CreationDate)
                .Skip((page - 1) * perPage)
                .Take(perPage)
                .Select(ct => new ThreadCard
                {
                    Id = ct.Id,
                    ClubId = ct.ClubId,
                    Title = ct.Title,
                    CreationDate = ct.CreationDate,
                    AuthorName = ct.Author.UserName,
                    AuthorAvatar = ct.Author.Avatar,
                    CommentsCount = ct.CommentsThread.Comments.Count
                })
                .AsNoTracking()
                .ToListAsync();
        }

        /// <summary>
        /// Get details of the desired thread
        /// </summary>
        /// <param name="id">ID of the thread</param>
        /// <returns>Returns `ThreadDetails` object</returns>
        public async Task<ThreadDetails> GetThreadDetails(long id)
        {
            return await _context.ClubThreads
                .Where(ct => ct.Id == id)
                .Select(ct => new ThreadDetails
                {
                    Id = ct.Id,
                    ClubId = ct.ClubId,
                    Title = ct.Title,
                    CreationDate = ct.CreationDate,
                    AuthorName = ct.Author.UserName,
                    AuthorAvatar = ct.Author.Avatar,
                    AuthorRole = ct.Author.Roles
                        .Where(ur => ur.Order.HasValue)
                        .OrderBy(ur => ur.Order)
                        .First(),
                    Body = ct.Body,
                    CommentsThread = ct.CommentsThread
                })
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }
    }
}