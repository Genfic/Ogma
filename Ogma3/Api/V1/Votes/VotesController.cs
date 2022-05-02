using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ogma3.Api.V1.Votes.Commands;
using Ogma3.Api.V1.Votes.Queries;

namespace Ogma3.Api.V1.Votes;

[Route("api/[controller]", Name = nameof(VotesController))]
[ApiController]
public class VotesController : ControllerBase
{
	private readonly IMediator _mediator;
	public VotesController(IMediator mediator) => _mediator = mediator;

	// GET api/votes/5
	[HttpGet("{storyId:long}")]
	public async Task<ActionResult<GetVotes.Result>> GetVotes(long storyId)
		=> await _mediator.Send(new GetVotes.Query(storyId));

	// POST api/votes
	[HttpPost]
	[Authorize]
	public async Task<ActionResult<CreateVote.Result>> PostVote([FromBody] CreateVote.Command data)
		=> await _mediator.Send(data);

	[HttpDelete]
	[Authorize]
	public async Task<ActionResult<DeleteVote.Result>> DeleteVote([FromBody] DeleteVote.Command data)
		=> await _mediator.Send(data);
}