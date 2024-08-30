using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using Ogma3.Api.V1.Subscriptions.Commands;
using Ogma3.Api.V1.Subscriptions.Queries;

namespace Ogma3.Api.V1.Subscriptions;

[Route("api/[controller]", Name = nameof(SubscriptionsController))]
[ApiController]
public class SubscriptionsController(IMediator mediator) : ControllerBase
{

	[HttpGet("thread")]
	public async Task<ActionResult<bool>> IsSubscribedToThreadAsync([FromQuery] GetSubscriptionStatus.Query data)
		=> await mediator.Send(data);

	[HttpPost("thread")]
	[Authorize]
	public async Task<ActionResult<bool>> SubscribeThreadAsync(SubscribeCommentsThread.Command data)
		=> await mediator.Send(data);

	[HttpDelete("thread")]
	[Authorize]
	public async Task<ActionResult<bool>> UnsubscribeThreadAsync(UnsubscribeCommentsThread.Command data)
		=> await mediator.Send(data);

	// Don't delete or this whole controller will break
	[HttpGet, OpenApiIgnore]
	public string Ping() => "Pong";
}