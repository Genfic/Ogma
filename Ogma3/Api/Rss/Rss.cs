using System.Threading.Tasks;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Ogma3.Api.Rss.Queries;
using Ogma3.Infrastructure.Formatters;

namespace Ogma3.Api.Rss;

[Route("rss", Name = nameof(RssController))]
public class RssController(IMediator mediator) : ControllerBase
{
	private const int Cache = 2400;
	
	[ResponseCache(Duration = Cache)]
	[HttpGet]
	[EnableRateLimiting("rss")]
	public async Task<ActionResult<RssResult>> Stories()
		=> await mediator.Send(new GetStories.Query());

	[ResponseCache(Duration = Cache)]
	[HttpGet("stories")]
	[EnableRateLimiting("rss")]
	public async Task<ActionResult<RssResult>> GetStoriesAsync()
		=> await mediator.Send(new GetStories.Query());

	[ResponseCache(Duration = Cache)]
	[HttpGet("story/{storyId:long}/chapters")]
	[EnableRateLimiting("rss")]
	public async Task<ActionResult<RssResult>> GetChapters(long storyId)
		=> await mediator.Send(new GetChapters.Query(storyId));

	[ResponseCache(Duration = Cache)]
	[HttpGet("blogposts")]
	[EnableRateLimiting("rss")]
	public async Task<ActionResult<RssResult>> GetBlogpostsAsync()
		=> await mediator.Send(new GetBlogposts.Query());
}