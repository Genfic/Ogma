using System.Threading.Tasks;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ogma3.Api.V1.ClubJoin.Commands;
using Ogma3.Api.V1.ClubJoin.Queries;

namespace Ogma3.Api.V1.ClubJoin;

[Route("api/[controller]", Name = nameof(ClubJoinController))]
[ApiController]
public class ClubJoinController : ControllerBase
{
	private readonly IMediator _mediator;
	public ClubJoinController(IMediator mediator) => _mediator = mediator;

	// GET api/clubjoin/5
	[HttpGet("{club:long}")]
	[Authorize]
	public async Task<ActionResult<bool>> CheckMembershipStatus(long club)
		=> await _mediator.Send(new GetClubMembershipStatus.Query(club));

	// POST api/clubjoin
	[HttpPost]
	[Authorize]
	[IgnoreAntiforgeryToken]
	public async Task<ActionResult<bool>> JoinClub(JoinClub.Command data)
		=> await _mediator.Send(data);

	[HttpDelete]
	[Authorize]
	[IgnoreAntiforgeryToken]
	public async Task<ActionResult<bool>> LeaveClub(LeaveClub.Command data)
		=> await _mediator.Send(data);
}