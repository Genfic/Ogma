using System.Collections.Generic;
using System.Threading.Tasks;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ogma3.Api.V1.Roles.Commands;
using Ogma3.Api.V1.Roles.Queries;
using Ogma3.Data.Roles;
using Ogma3.Infrastructure.Constants;

namespace Ogma3.Api.V1.Roles;

[Route("api/[controller]", Name = nameof(RolesController))]
[ApiController]
public class RolesController : ControllerBase
{
	private readonly IMediator _mediator;
	public RolesController(IMediator mediator) => _mediator = mediator;

	// GET: api/Roles
	[HttpGet]
	public async Task<ActionResult<List<RoleDto>>> GetRoles()
		=> await _mediator.Send(new GetAllRoles.Query());

	[HttpGet("{id:long}")]
	public async Task<ActionResult<RoleDto>> GetRole(long id)
		=> await _mediator.Send(new GetRoleById.Query(id));

	// PUT: api/Namespaces/5
	[HttpPut]
	[Authorize(Roles = RoleNames.Admin)]
	public async Task<ActionResult<RoleDto>> PutRole(UpdateRole.Command data)
		=> await _mediator.Send(data);

	// POST: api/Roles
	[HttpPost]
	[Authorize(Roles = RoleNames.Admin)]
	public async Task<ActionResult<RoleDto>> PostRole(CreateRole.Command data)
		=> await _mediator.Send(data);

	// DELETE: api/Roles/5
	[HttpDelete("{id:long}")]
	[Authorize(Roles = RoleNames.Admin)]
	[ProducesResponseType(StatusCodes.Status200OK), ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<ActionResult> DeleteRole(long id)
		=> await _mediator.Send(new DeleteRole.Command(id));
}