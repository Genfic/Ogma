using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Enums;
using Ogma3.Pages.Shared;

namespace Ogma3.Pages.Blog
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        public IndexModel(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        
        public IList<BlogpostCard> Posts { get;set; }
        public string SearchBy { get; set; }
        public EBlogpostSortingOptions SortBy { get; set; }

        private const int PerPage = 25;
        public Pagination Pagination { get; set; }

        public async Task<ActionResult> OnGetAsync([FromQuery] string q, [FromQuery] EBlogpostSortingOptions sort, [FromQuery] int page = 1)
        {
            SearchBy = q;
            SortBy = sort;
            
            var query = _context.Blogposts.AsQueryable();
            
            if (!string.IsNullOrEmpty(q))
            {
                // Search by tags
                var tags = q
                    .Split(' ')
                    .Where(x => x.StartsWith('#'))
                    .Select(x => x.ToLower().Trim())
                    .ToArray();
                if (tags.Length > 0)
                    query = query.Where(b => tags.All(i => b.Hashtags.Contains(i)));

                // Search in title
                var search = q
                    .Split(' ')
                    .Where(x => !x.StartsWith('#') && !string.IsNullOrEmpty(x))
                    .ToList();
                if (search.Count > 0)
                    query = query.Where(b => EF.Functions.Like(b.Title.ToUpper(), $"%{string.Join(' ', search)}%".ToUpper()));

            }
            
            // Save post count at this stage
            var postsCount = await query.CountAsync();
            
            // Sort
            query = sort switch
            {
                EBlogpostSortingOptions.TitleAscending  => query.OrderBy(s => s.Title),
                EBlogpostSortingOptions.TitleDescending => query.OrderByDescending(s => s.Title),
                EBlogpostSortingOptions.DateAscending   => query.OrderBy(s => s.PublishDate),
                EBlogpostSortingOptions.DateDescending  => query.OrderByDescending(s => s.PublishDate),
                EBlogpostSortingOptions.WordsAscending  => query.OrderBy(s => s.WordCount),
                EBlogpostSortingOptions.WordsDescending => query.OrderByDescending(s => s.WordCount),
                _ => query.OrderByDescending(s => s.WordCount)
            };
            
            // Finalize query
            Posts = await query
                .Include(b => b.Author)
                .Where(b => b.IsPublished)
                .Paginate(page, PerPage)
                .Take(PerPage)
                .ProjectTo<BlogpostCard>(_mapper.ConfigurationProvider)
                .AsNoTracking()
                .ToListAsync();
            
            // Prepare pagination model
            Pagination = new Pagination
            {
                PerPage = PerPage,
                ItemCount = postsCount,
                CurrentPage = page
            };
            
            return Page();
        }
    }
}