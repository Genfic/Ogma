using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using Ogma3.Api.V1.CommentThreads.Commands;
using Ogma3.Api.V1.CommentThreads.Queries;

namespace Ogma3.Api.V1.CommentThreads;

[Route("api/[controller]", Name = nameof(CommentsThreadController))]
[ApiController]
public class CommentsThreadController : ControllerBase
{
	private readonly IMediator _mediator;

	public CommentsThreadController(IMediator mediator) => _mediator = mediator;

	[HttpGet("permissions/{id:long}")]
	public async Task<GetPermissions.Result> GetPermissionsAsync(long id)
		=> await _mediator.Send(new GetPermissions.Query(id));


	[HttpGet("lock/status/{id:long}")]
	public async Task<ActionResult<bool>> GetLockStatusAsync(long id)
		=> await _mediator.Send(new GetLockStatus.Query(id));

	// POST
	[HttpPost("lock")]
	[Authorize]
	[IgnoreAntiforgeryToken]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<ActionResult<bool>> LockThreadAsync([FromBody] LockThread.Command command)
		=> await _mediator.Send(command);


	// Don't delete or this whole controller will break
	[HttpGet, OpenApiIgnore]
	public string Ping() => "Pong";

}