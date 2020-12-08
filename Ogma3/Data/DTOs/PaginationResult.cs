using System.Collections.Generic;

namespace Ogma3.Data.DTOs
{
    /// <summary>
    /// Object to hold the results of a paginated fetch
    /// </summary>
    /// <typeparam name="T">Type of objects to hold</typeparam>
    public class PaginationResult<T>
    {
        /// <summary>
        /// List of the fetched elements
        /// </summary>
        public IEnumerable<T> Elements { get; set; }
        
        /// <summary>
        /// Total number of those elements, not just the number of them on the page
        /// </summary>
        public int Total { get; set; }
        
        /// <summary>
        /// Number of elements per page
        /// </summary>
        public int PerPage { get; set; }
        
        /// <summary>
        /// Number of pages the total number of elements can be divided into
        /// </summary>
        public int Pages { get; set; }
        
        /// <summary>
        /// Number of the requested page
        /// </summary>
        public int Page { get; set; }
    }
}