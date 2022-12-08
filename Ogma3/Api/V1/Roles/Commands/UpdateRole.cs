using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Ogma3.Data.Roles;
using Ogma3.Infrastructure.MediatR.Bases;

namespace Ogma3.Api.V1.Roles.Commands;

public static class UpdateRole
{
	public sealed record Command(long Id, string Name, bool IsStaff, string Color, byte? Order) : IRequest<ActionResult<RoleDto>>;

	public class CommandValidator : AbstractValidator<Command>
	{
		public CommandValidator() => RuleFor(r => r.Name).NotEmpty();
	}

	public class Handler : BaseHandler, IRequestHandler<Command, ActionResult<RoleDto>>
	{
		private readonly RoleManager<OgmaRole> _roleManager;
		public Handler(RoleManager<OgmaRole> roleManager) => _roleManager = roleManager;

		public async Task<ActionResult<RoleDto>> Handle(Command request, CancellationToken cancellationToken)
		{
			var (id, name, isStaff, color, order) = request;

			var role = await _roleManager.FindByIdAsync(id.ToString());

			if (role is null) return NotFound();

			role.Name = name;
			role.IsStaff = isStaff;
			role.Color = color;
			role.Order = order;

			await _roleManager.UpdateAsync(role);

			return CreatedAtAction(
				nameof(RolesController.GetRole),
				nameof(RolesController)[..^10],
				new { role.Id },
				role
			);
		}
	}
}