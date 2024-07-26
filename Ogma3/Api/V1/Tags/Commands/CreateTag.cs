using FluentValidation;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Tags;
using Ogma3.Infrastructure.Mediator.Bases;

namespace Ogma3.Api.V1.Tags.Commands;

public static class CreateTag
{
	public sealed record Command(string Name, string? Description, ETagNamespace? Namespace) : IRequest<ActionResult>;

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
			var (name, description, ns) = request;

			var tagExist = await _context.Tags
				.Where(t => t.Name == name && t.Namespace == ns)
				.AnyAsync(cancellationToken);

			if (tagExist) return Conflict($"Tag {name} already exists in the {ns} namespace.");

			var entity = _context.Tags.Add(new Tag
			{
				Name = name,
				Description = description,
				Namespace = ns
			});
			await _context.SaveChangesAsync(cancellationToken);

			return CreatedAtAction(
				nameof(TagsController.GetTag),
				nameof(TagsController)[..^10],
				new { entity.Entity.Id },
				entity.Entity
			);
		}
	}
}