using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ogma3.Api.V1.Notifications.Commands;
using Ogma3.Api.V1.Notifications.Queries;

namespace Ogma3.Api.V1.Notifications;

[Route("api/[controller]", Name = nameof(NotificationsController))]
[ApiController]
public class NotificationsController : ControllerBase
{
	private readonly IMediator _mediator;
	public NotificationsController(IMediator mediator) => _mediator = mediator;

	[HttpGet]
	[Authorize]
	public async Task<ActionResult<List<GetUserNotifications.Result>>> GetUserNotifications()
		=> await _mediator.Send(new GetUserNotifications.Query());

	[HttpGet("count")]
	[Authorize]
	public async Task<ActionResult<int>> CountUserNotifications()
		=> await _mediator.Send(new CountUserNotifications.Query());

	[HttpDelete("{id:long}")]
	[Authorize]
	[ProducesResponseType(StatusCodes.Status200OK), ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<ActionResult> DeleteNotification(long id)
		=> await _mediator.Send(new DeleteNotification.Command(id));
}