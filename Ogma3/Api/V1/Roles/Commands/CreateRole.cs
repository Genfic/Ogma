using FluentValidation;
using Mediator;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Ogma3.Data.Roles;
using Ogma3.Infrastructure.Mediator.Bases;

namespace Ogma3.Api.V1.Roles.Commands;

public static class CreateRole
{
	public sealed record Command(string Name, bool IsStaff, string Color, byte? Order) : IRequest<ActionResult<RoleDto>>;

	public class CommandValidator : AbstractValidator<Command>
	{
		public CommandValidator() => RuleFor(r => r.Name).NotEmpty();
	}

	public class Handler : BaseHandler, IRequestHandler<Command, ActionResult<RoleDto>>
	{
		private readonly RoleManager<OgmaRole> _roleManager;
		public Handler(RoleManager<OgmaRole> roleManager) => _roleManager = roleManager;

		public async ValueTask<ActionResult<RoleDto>> Handle(Command request, CancellationToken cancellationToken)
		{
			var (name, isStaff, color, order) = request;
			if (await _roleManager.RoleExistsAsync(name)) return Conflict($"Role {name} already exists");

			var role = new OgmaRole
			{
				Name = name,
				IsStaff = isStaff,
				Color = color,
				Order = order
			};

			await _roleManager.CreateAsync(role);

			return CreatedAtAction(
				nameof(RolesController.GetRole),
				nameof(RolesController)[..^10],
				new { role.Id },
				role
			);
		}
	}
}