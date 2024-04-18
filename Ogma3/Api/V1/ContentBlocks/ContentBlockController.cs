using System.Threading.Tasks;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using Ogma3.Api.V1.ContentBlocks.Commands;
using Ogma3.Infrastructure.Constants;

namespace Ogma3.Api.V1.ContentBlocks;

[Route("api/[controller]", Name = nameof(ContentBlockController))]
[ApiController]
[Authorize(Roles = $"{RoleNames.Admin},{RoleNames.Moderator}")]
public class ContentBlockController(IMediator mediator) : ControllerBase
{
	[HttpPost]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status200OK)]
	public async Task<ActionResult> Block(BlockContent.Command data)
		=> await mediator.Send(data);

	// Don't delete or this whole controller will break
	[HttpGet, OpenApiIgnore]
	public string Ping() => "Pong";
}