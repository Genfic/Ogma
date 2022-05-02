using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using Ogma3.Api.V1.UserActivity.Commands;

namespace Ogma3.Api.V1.UserActivity;

[Route("api/[controller]", Name = nameof(UserActivityController))]
[ApiController]
public class UserActivityController : ControllerBase
{
	private readonly IMediator _mediator;
	public UserActivityController(IMediator mediator) => _mediator = mediator;

	// POST
	[HttpHead]
	[ProducesResponseType(StatusCodes.Status200OK)]
	public async Task<OkResult> UpdateLastActiveAsync()
		=> await _mediator.Send(new UpdateLastActive.Command());

	// Don't delete or this whole controller will break
	[HttpGet, OpenApiIgnore]
	public string Ping() => "Pong";
}