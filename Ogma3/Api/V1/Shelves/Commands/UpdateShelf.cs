using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Shelves;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Infrastructure.Mediator.Bases;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.Shelves.Commands;

public static class UpdateShelf
{
	public sealed record Command(
		long Id,
		string Name,
		string Description,
		bool IsQuickAdd,
		bool IsPublic,
		bool TrackUpdates,
		string Color,
		long Icon
	) : IRequest<ActionResult<ShelfDto>>;

	public class CommandValidator : AbstractValidator<Command>
	{
		public CommandValidator()
		{
			RuleFor(s => s.Id)
				.NotEmpty();
			RuleFor(s => s.Name)
				.MaximumLength(CTConfig.CShelf.MaxNameLength)
				.MinimumLength(CTConfig.CShelf.MinNameLength);
			RuleFor(s => s.Description)
				.MaximumLength(CTConfig.CShelf.MaxDescriptionLength);
			RuleFor(s => s.Color)
				.Length(7);
		}
	}

	public class Handler(ApplicationDbContext context, IUserService userService)
		: BaseHandler, IRequestHandler<Command, ActionResult<ShelfDto>>
	{
		public async ValueTask<ActionResult<ShelfDto>> Handle(Command request, CancellationToken cancellationToken)
		{
			if (userService.User?.GetNumericId() is not {} uid) return Unauthorized();

			var (id, name, description, isQuickAdd, isPublic, trackUpdates, color, icon) = request;

			var shelf = await context.Shelves
				.Where(s => s.Id == id)
				.FirstOrDefaultAsync(cancellationToken);

			if (shelf is null) return NotFound();
			if (shelf.OwnerId != uid) return Unauthorized();

			shelf.Name = name;
			shelf.Description = description;
			shelf.IsQuickAdd = isQuickAdd;
			shelf.IsPublic = isPublic;
			shelf.TrackUpdates = trackUpdates;
			shelf.Color = color;
			shelf.IconId = icon;

			await context.SaveChangesAsync(cancellationToken);

			// TODO: Do we need this? Without it, we could just use .ExecuteUpdateAsync() instead of multiple db calls
			return CreatedAtAction(
				nameof(ShelvesController.GetShelf),
				nameof(ShelvesController)[..^10],
				new { shelf.Id },
				shelf
			);
		}
	}
}