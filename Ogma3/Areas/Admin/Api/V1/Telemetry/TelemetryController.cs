using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ogma3.Areas.Admin.Api.V1.Telemetry.Queries;

namespace Ogma3.Areas.Admin.Api.V1.Telemetry;

[Route("admin/api/[controller]", Name = nameof(TelemetryController))]
[ApiController]
[Authorize]
public class TelemetryController(IMediator mediator) : ControllerBase
{
	[HttpGet(nameof(GetTableInfo))]
	public async Task<ActionResult<List<GetTableInfo.Response>>> GetTableInfo()
		=> await mediator.Send(new GetTableInfo.Query());

	[HttpGet(nameof(GetImportantItemCounts))]
	public async Task<ActionResult<Dictionary<string, int>>> GetImportantItemCounts()
		=> await mediator.Send(new GetImportantItemCounts.Query());
}