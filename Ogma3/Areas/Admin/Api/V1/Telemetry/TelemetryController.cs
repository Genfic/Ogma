using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ogma3.Areas.Admin.Api.V1.Telemetry.Queries;

namespace Ogma3.Areas.Admin.Api.V1.Telemetry;

[Route("admin/api/[controller]", Name = nameof(TelemetryController))]
[ApiController]
[Authorize]
public class TelemetryController : ControllerBase
{
	private readonly IMediator _mediator;
	public TelemetryController(IMediator mediator) => _mediator = mediator;

	[HttpGet(nameof(GetTableInfo))]
	public async Task<ActionResult<List<GetTableInfo.Response>>> GetTableInfo()
		=> await _mediator.Send(new GetTableInfo.Query());

	[HttpGet(nameof(GetImportantItemCounts))]
	public async Task<ActionResult<Dictionary<string, int>>> GetImportantItemCounts()
		=> await _mediator.Send(new GetImportantItemCounts.Query());
}