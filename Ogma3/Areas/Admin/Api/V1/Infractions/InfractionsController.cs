using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ogma3.Areas.Admin.Api.V1.Infractions.Commands;
using Ogma3.Areas.Admin.Api.V1.Infractions.Queries;
using Ogma3.Infrastructure.Constants;

namespace Ogma3.Areas.Admin.Api.V1.Infractions;

[Route("api/[controller]", Name = nameof(InfractionsController))]
[ApiController]
[Authorize(Roles = $"{RoleNames.Admin},{RoleNames.Moderator}")]
public class InfractionsController(IMediator mediator) : ControllerBase
{
	[HttpGet]
	public async Task<ActionResult<List<GetUserInfractions.Result>>> GetInfractionsAsync([FromQuery] GetUserInfractions.Query query)
		=> await mediator.Send(query);

	[HttpGet("details")]
	public async Task<ActionResult<GetInfractionDetails.Result>> GetInfractionDetails([FromQuery] GetInfractionDetails.Query query)
		=> await mediator.Send(query);

	[HttpPost]
	public async Task<ActionResult<CreateInfraction.Response>> AddInfraction(CreateInfraction.Command command)
		=> await mediator.Send(command);

	[HttpDelete("{InfractionId:long}")]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(typeof(DeactivateInfraction.Response), StatusCodes.Status200OK)]
	public async Task<ActionResult<DeactivateInfraction.Response>> DeactivateInfraction([FromRoute] DeactivateInfraction.Command command)
		=> await mediator.Send(command);
}