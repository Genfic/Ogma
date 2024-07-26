using Mediator;
using Microsoft.AspNetCore.Mvc;
using Ogma3.Api.V1.ChaptersReads.Commands;
using Ogma3.Api.V1.ChaptersReads.Queries;

namespace Ogma3.Api.V1.ChaptersReads;

[Route("api/[controller]", Name = nameof(ChaptersReadController))]
[ApiController]
public class ChaptersReadController(IMediator mediator) : ControllerBase
{
	// GET api/chaptersread/5
	[HttpGet("{story:long}")]
	public async Task<ActionResult<HashSet<long>>> GetChaptersRead(long story)
		=> await mediator.Send(new GetReadChapters.Query(story));

	// POST api/chaptersread
	[HttpPost]
	public async Task<ActionResult<HashSet<long>>> PostChaptersRead(MarkChapterAsRead.Command post)
		=> await mediator.Send(post);

	[HttpDelete]
	public async Task<ActionResult<HashSet<long>>> DeleteChaptersRead(MarkChapterAsUnread.Command post)
		=> await mediator.Send(post);
}