using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ogma3.Api.V1.Infractions.Commands;
using Ogma3.Api.V1.Infractions.Queries;
using Ogma3.Infrastructure.Constants;

namespace Ogma3.Api.V1.Infractions;

[Route("api/[controller]", Name = nameof(InfractionsController))]
[ApiController]
[Authorize(Roles = $"{RoleNames.Admin},{RoleNames.Moderator}")]
public class InfractionsController : ControllerBase
{
	private readonly IMediator _mediator;
	public InfractionsController(IMediator mediator) => _mediator = mediator;

	[HttpGet]
	public async Task<ActionResult<List<GetUserInfractions.Result>>> GetInfractionsAsync([FromQuery] long id)
		=> await _mediator.Send(new GetUserInfractions.Query(id));

	[HttpGet("details")]
	public async Task<ActionResult<GetInfractionDetails.Result>> GetInfractionDetails([FromQuery] long id)
		=> await _mediator.Send(new GetInfractionDetails.Query(id));

	[HttpPost]
	public async Task<ActionResult<CreateInfraction.Response>> AddInfraction(CreateInfraction.Command command)
		=> await _mediator.Send(command);

	[HttpDelete("{id:long}")]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(typeof(DeactivateInfraction.Response), StatusCodes.Status200OK)]
	public async Task<ActionResult<DeactivateInfraction.Response>> DeactivateInfraction(long id)
		=> await _mediator.Send(new DeactivateInfraction.Command(id));
}