using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using Ogma3.Api.V1.Users.Commands;
using Ogma3.Api.V1.Users.Queries;
using Ogma3.Infrastructure.Constants;

namespace Ogma3.Api.V1.Users;

[Route("api/[controller]", Name = nameof(UsersController))]
[ApiController]
[Authorize]
public class UsersController : ControllerBase
{
	private readonly IMediator _mediator;
	public UsersController(IMediator mediator) => _mediator = mediator;

	// api/Users/block
	[HttpPost("block")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	public async Task<ActionResult<bool>> BlockUser(BlockUser.Command command)
		=> await _mediator.Send(command);

	[HttpDelete("block")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	public async Task<ActionResult<bool>> UnblockUser(UnblockUser.Command command)
		=> await _mediator.Send(command);

	// api/Users/follow
	[HttpPost("follow")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	public async Task<ActionResult<bool>> FollowUser(FollowUser.Command command)
		=> await _mediator.Send(command);

	[HttpDelete("follow")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	public async Task<ActionResult<bool>> UnfollowUser(UnfollowUser.Command command)
		=> await _mediator.Send(command);

	[HttpPost("roles")]
	[Authorize(Roles = RoleNames.Admin)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status200OK)]
	public async Task<IActionResult> ManageRoles(UpdateRoles.Command command)
		=> await _mediator.Send(command);

	[HttpGet("names")]
	[Authorize(Roles = $"{RoleNames.Admin}, {RoleNames.Moderator}, {RoleNames.Helper}")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
	public async Task<ActionResult<List<string>>> FindNames([FromQuery] FindName.Query query)
		=> await _mediator.Send(query);


	// Don't delete or this whole controller will break
	[HttpGet, OpenApiIgnore]
	public string Ping() => "Pong";
}