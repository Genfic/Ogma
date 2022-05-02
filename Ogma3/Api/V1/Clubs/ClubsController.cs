using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using Ogma3.Api.V1.Clubs.Commands;
using Ogma3.Api.V1.Clubs.Queries;

namespace Ogma3.Api.V1.Clubs;

[Route("api/[controller]", Name = nameof(ClubsController))]
[ApiController]
public class ClubsController : ControllerBase
{
	private readonly IMediator _mediator;
	public ClubsController(IMediator mediator) => _mediator = mediator;

	// GET: /api/clubs/user
	[HttpGet("user")]
	[Authorize]
	public async Task<ActionResult<List<GetJoinedClubs.Response>>> GetUserClubs()
		=> await _mediator.Send(new GetJoinedClubs.Query());

	// GET: /api/clubs/story/3
	[HttpGet("story/{id:long}")]
	public async Task<ActionResult<List<GetClubsWithStory.Result>>> GetClubsWithStory(long id)
		=> await _mediator.Send(new GetClubsWithStory.Query(id));

	[HttpPost("user/ban")]
	[IgnoreAntiforgeryToken]
	public async Task<ActionResult<bool>> BanUser(BanUser.Command command)
		=> await _mediator.Send(command);

	[HttpDelete("user/ban")]
	[IgnoreAntiforgeryToken]
	public async Task<ActionResult<bool>> UnbanUser(UnbanUser.Command command)
		=> await _mediator.Send(command);

	// Don't delete or this whole controller will break
	[HttpGet, OpenApiIgnore]
	public string Ping() => "Pong";
}