#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data.Models;

namespace Ogma3.Data.Repositories
{
    public class BaseRepository<T> where T : BaseModel
    {
        private ApplicationDbContext Context { get; }

        public BaseRepository(ApplicationDbContext context)
        {
            Context = context;
        }

        /// <summary>
        /// Get all entities of type T
        /// </summary>
        /// <returns>Collection of all results</returns>
        public async Task<ICollection<T>> GetAll()
        {
            return await Context.Set<T>()
                .ToListAsync();
        }

        /// <summary>
        /// Get a limited, paged number of entities of type T
        /// </summary>
        /// <param name="page">Page number, has to be 1 or higher</param>
        /// <param name="perPage">Number of entities per page, has to be 1 or higher</param>
        /// <returns>Collection of all results</returns>
        public async Task<ICollection<T>> GetPage(int page = 1, int perPage = 10)
        {
            page = Math.Max(1, page);
            perPage = Math.Max(1, perPage);
            
            return await Context.Set<T>()
                .Skip((page - 1) * perPage)
                .Take(perPage)
                .ToListAsync();
        }

        /// <summary>
        /// Gets an entity of type T by its ID
        /// </summary>
        /// <param name="id">ID of the entity to get</param>
        /// <returns>The entity</returns>
        public async Task<T> Get(long id)
        {
            return await Context.Set<T>()
                .FindAsync(id);
        }

        /// <summary>
        /// Finds all entities of type T that match the predicate
        /// </summary>
        /// <param name="filter">Predicate to filter by</param>
        /// <returns>Collection of all results</returns>
        public async Task<ICollection<T>> Find(Expression<Func<T, bool>> filter)
        {
            return await Context.Set<T>()
                .Where(filter)
                .ToListAsync();
        }

        /// <summary>
        /// Creates an entity of type T
        /// </summary>
        /// <param name="entity">Entity to be created</param>
        /// <returns>The created entity</returns>
        public async Task<T> Create(T entity)
        {
            await Context.Set<T>()
                .AddAsync(entity);
            return entity;
        }

        /// <summary>
        /// Updates an entity of type T
        /// </summary>
        /// <param name="entity">Entity to be updated</param>
        /// <returns>The updated entity</returns>
        public T Update(T entity)
        {
            Context.Set<T>()
                .Update(entity);
            return entity;
        }

        /// <summary>
        /// Deletes an entity of type T
        /// </summary>
        /// <param name="entity">Entity to be deleted</param>
        /// <returns>The deleted entity</returns>
        public T Delete(T entity)
        {
            Context.Set<T>().Remove(entity);
            return entity;
        }
    }
}