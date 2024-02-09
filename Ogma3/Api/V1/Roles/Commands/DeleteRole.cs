using System.Threading;
using System.Threading.Tasks;
using Mediator;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Ogma3.Data.Roles;
using Ogma3.Infrastructure.Mediator.Bases;

namespace Ogma3.Api.V1.Roles.Commands;

public static class DeleteRole
{
	public sealed record Command(long Id) : IRequest<ActionResult>;

	public class Handler : BaseHandler, IRequestHandler<Command, ActionResult>
	{
		private readonly RoleManager<OgmaRole> _roleManager;
		public Handler(RoleManager<OgmaRole> roleManager) => _roleManager = roleManager;

		public async ValueTask<ActionResult> Handle(Command request, CancellationToken cancellationToken)
		{
			var role = await _roleManager.FindByIdAsync(request.Id.ToString());

			if (role is null) return NotFound();

			await _roleManager.DeleteAsync(role);

			return Ok(role.Id);
		}
	}
}