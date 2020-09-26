using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Ogma3.Data;

namespace Ogma3.Infrastructure.TagHelpers
{
    public class StoryCountTagHelper: TagHelper
    {
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _cache;

        public StoryCountTagHelper(ApplicationDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        /// <summary>
        /// How often should the cache refresh in minutes
        /// </summary>
        private int CacheTime { get; set; } = 60;

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            const string name = nameof(StoryCountTagHelper) + "_cache";
            
            int count;
            if (_cache.TryGetValue(name, out int c))
            {
                count = c;
            }
            else
            {
                count = await _context.Stories
                    .Where(s => s.IsPublished)
                    .CountAsync();
                _cache.Set(name, count, TimeSpan.FromMinutes(CacheTime));
            }

            output.TagName = "span";
            output.Content.SetContent(count.ToString());
        }
    }
}