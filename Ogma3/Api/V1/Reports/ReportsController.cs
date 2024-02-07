using System.Threading.Tasks;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using Ogma3.Api.V1.Reports.Commands;
using Ogma3.Infrastructure.Attributes;

namespace Ogma3.Api.V1.Reports;

[Route("api/[controller]", Name = nameof(ReportsController))]
[ApiController]
public class ReportsController : ControllerBase
{
	private readonly IMediator _mediator;
	public ReportsController(IMediator mediator) => _mediator = mediator;

	// POST
	[HttpPost]
	[Authorize]
	[Throttle(Count = 3, TimeUnit = TimeUnit.Hour)]
	public async Task<ActionResult<long>> PostReportsAsync([FromBody] ReportContent.Command data)
		=> await _mediator.Send(data);

	// Don't delete or this whole controller will break
	[HttpGet, OpenApiIgnore]
	public string Ping() => "Pong";
}