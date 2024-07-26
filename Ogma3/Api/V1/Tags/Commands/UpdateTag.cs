using FluentValidation;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Tags;
using Ogma3.Infrastructure.Mediator.Bases;

namespace Ogma3.Api.V1.Tags.Commands;

public static class UpdateTag
{
	public sealed record Command(long Id, string? Name, string? Description, ETagNamespace? Namespace) : IRequest<ActionResult>;

	public class CommandValidator : AbstractValidator<Command>
	{
		public CommandValidator()
		{
			RuleFor(t => t.Name)
				.MinimumLength(CTConfig.CTag.MinNameLength)
				.MaximumLength(CTConfig.CTag.MaxNameLength);
			RuleFor(t => t.Description)
				.MaximumLength(CTConfig.CTag.MaxDescLength);
		}
	}

	public class Handler : BaseHandler, IRequestHandler<Command, ActionResult>
	{
		private readonly ApplicationDbContext _context;
		public Handler(ApplicationDbContext context) => _context = context;

		public async ValueTask<ActionResult> Handle(Command request, CancellationToken cancellationToken)
		{
			var (id, name, description, ns) = request;

			// Different ID but the same (name, namespace) tuple means update would make the tag a duplicate
			var duplicateExists = await _context.Tags
				.Where(t => t.Id != id)
				.Where(t => t.Name == name && t.Namespace == ns)
				.AnyAsync(cancellationToken);

			if (duplicateExists) return Conflict($"Tag {name} already exists in the {ns} namespace.");

			var res = await _context.Tags
				.Where(t => t.Id == id)
				.ExecuteUpdateAsync(tag => tag
						.SetProperty(t => t.Name, t => name ?? t.Name)
						.SetProperty(t => t.Description, description)
						.SetProperty(t => t.Namespace, ns),
					cancellationToken);

			await _context.SaveChangesAsync(cancellationToken);

			return res > 0 ? Ok() : NotFound();
		}
	}
}