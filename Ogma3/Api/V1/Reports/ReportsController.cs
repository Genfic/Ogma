using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using Ogma3.Api.V1.Reports.Commands;
using Ogma3.Infrastructure.Attributes;

namespace Ogma3.Api.V1.Reports;

[Route("api/[controller]", Name = nameof(ReportsController))]
[ApiController]
public class ReportsController(IMediator mediator) : ControllerBase
{
	// POST
	[HttpPost]
	[Authorize]
	[Throttle(Count = 3, TimeUnit = TimeUnit.Hour)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status200OK)]
	public async Task<ActionResult<long>> PostReportsAsync([FromBody] ReportContent.Command data)
		=> await mediator.Send(data);

	// Don't delete or this whole controller will break
	[HttpGet, OpenApiIgnore]
	public string Ping() => "Pong";
}