using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Ogma3.Services.RssService;

namespace Ogma3.Rss
{
    [Route("rss", Name = nameof(RssController))]
    [ApiController]
    public class RssController : Controller
    {
        private const int Cache = 2400;
        private readonly IRssService _rss;

        public RssController(IRssService rss) => _rss = rss;

        [ResponseCache(Duration = Cache)]
        [HttpGet]
        public async Task<IActionResult> Stories() => Ok(await _rss.GetStoriesAsync());

        [ResponseCache(Duration = Cache)]
        [HttpGet("stories")]
        public async Task<IActionResult> GetStoriesAsync() => Ok(await _rss.GetStoriesAsync());

        [ResponseCache(Duration = Cache)]
        [HttpGet("blogposts")]
        public async Task<IActionResult> GetBlogpostsAsync() => Ok(await _rss.GetBlogpostsAsync());
    }
}