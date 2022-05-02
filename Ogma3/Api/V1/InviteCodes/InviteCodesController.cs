using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ogma3.Api.V1.InviteCodes.Commands;
using Ogma3.Api.V1.InviteCodes.Queries;
using Ogma3.Data.InviteCodes;
using Ogma3.Infrastructure.Constants;

namespace Ogma3.Api.V1.InviteCodes;

[ApiController]
[Route("api/[controller]", Name = nameof(InviteCodesController))]
public class InviteCodesController : ControllerBase
{
	private readonly IMediator _mediator;
	public InviteCodesController(IMediator mediator) => _mediator = mediator;

	// GET: api/InviteCodes
	[HttpGet]
	[Authorize]
	public async Task<ActionResult<List<InviteCodeDto>>> GetInviteCodes()
		=> await _mediator.Send(new GetIssuedInviteCodes.Query());

	// GET: api/InviteCodes/paginated?page=1&perPage=10
	[HttpGet("paginated")]
	[Authorize(Roles = $"{RoleNames.Admin}, {RoleNames.Moderator}")]
	public async Task<ActionResult<List<InviteCodeDto>>> GetPaginatedInviteCodes([FromQuery] GetPaginatedInviteCodes.Query query)
		=> await _mediator.Send(query);

	[HttpPost]
	[Authorize]
	public async Task<ActionResult<InviteCodeDto>> PostInviteCode()
		=> await _mediator.Send(new IssueInviteCode.Command());


	[HttpPost("no-limit")]
	[Authorize(Roles = $"{RoleNames.Admin}, {RoleNames.Moderator}")]
	public async Task<ActionResult<InviteCodeDto>> PostInviteCodeNoLimit()
		=> await _mediator.Send(new AdminIssueInviteCode.Command());


	[HttpDelete("{id:long}")]
	[Authorize(Roles = RoleNames.Admin)]
	public async Task<ActionResult<long>> DeleteInviteCode(long id)
		=> await _mediator.Send(new DeleteInviteCode.Command(id));
}