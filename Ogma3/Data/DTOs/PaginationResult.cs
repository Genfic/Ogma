using System.Collections.Generic;

namespace Ogma3.Data.DTOs
{
    // TODO: Consider using records?
    public class PaginationResult<T>
    {
        public IEnumerable<T> Elements { get; set; }
        public int Total { get; set; }
        public int PerPage { get; set; }
        public int Pages { get; set; }
        public int Page { get; set; }
    }
}