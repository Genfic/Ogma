using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using Ogma3.Infrastructure.Formatters;
using Ogma3.Services.RssService;

namespace Ogma3.Rss;

[Route("rss", Name = nameof(RssController))]
public class RssController : Controller
{
	private const int Cache = 2400;
	private readonly IRssService _rss;

	public RssController(IRssService rss) => _rss = rss;

	[ResponseCache(Duration = Cache)]
	[HttpGet]
	[OpenApiIgnore]
	public async Task<ActionResult<RssResult>> Stories() => Ok(await _rss.GetStoriesAsync());

	[ResponseCache(Duration = Cache)]
	[HttpGet("stories")]
	[OpenApiIgnore]
	public async Task<ActionResult<RssResult>> GetStoriesAsync() => Ok(await _rss.GetStoriesAsync());

	[ResponseCache(Duration = Cache)]
	[HttpGet("blogposts")]
	[OpenApiIgnore]
	public async Task<ActionResult<RssResult>> GetBlogpostsAsync() => Ok(await _rss.GetBlogpostsAsync());
}