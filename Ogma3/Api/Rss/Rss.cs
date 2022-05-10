using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ogma3.Api.Rss.Queries;
using Ogma3.Infrastructure.Formatters;

namespace Ogma3.Api.Rss;

[Route("rss", Name = nameof(RssController))]
public class RssController : ControllerBase
{
	private const int Cache = 2400;
	private readonly IMediator _mediator;

	public RssController(IMediator mediator) => _mediator = mediator;

	[ResponseCache(Duration = Cache)]
	[HttpGet]
	public async Task<ActionResult<RssResult>> Stories()
		=> await _mediator.Send(new GetStories.Query());

	[ResponseCache(Duration = Cache)]
	[HttpGet("stories")]
	public async Task<ActionResult<RssResult>> GetStoriesAsync()
		=> await _mediator.Send(new GetStories.Query());

	[ResponseCache(Duration = Cache)]
	[HttpGet("story/{storyId:long}/chapters")]
	public async Task<ActionResult<RssResult>> GetChapters(long storyId)
		=> await _mediator.Send(new GetChapters.Query(storyId));

	[ResponseCache(Duration = Cache)]
	[HttpGet("blogposts")]
	public async Task<ActionResult<RssResult>> GetBlogpostsAsync()
		=> await _mediator.Send(new GetBlogposts.Query());
}