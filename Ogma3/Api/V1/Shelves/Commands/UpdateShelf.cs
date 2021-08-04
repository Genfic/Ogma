using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using MediatR;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Ogma3.Data.Shelves;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.Shelves.Commands
{
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

        public class Handler : IRequestHandler<Command, ActionResult<ShelfDto>>
        {
            private readonly ApplicationDbContext _context;
            private readonly long? _uid;
            public Handler(ApplicationDbContext context, IUserService userService)
            {
                _context = context;
                _uid = userService.User?.GetNumericId();
            }

            public async Task<ActionResult<ShelfDto>> Handle(Command request, CancellationToken cancellationToken)
            {
                if (_uid is null) return new UnauthorizedResult();
                
                var (id, name, description, isQuickAdd, isPublic, trackUpdates, color, icon) = request;

                var shelf = await _context.Shelves
                    .Where(s => s.Id == id)
                    .FirstOrDefaultAsync(cancellationToken);

                if (shelf.OwnerId != _uid) return new UnauthorizedResult();
                
                shelf.Name = name;
                shelf.Description = description;
                shelf.IsQuickAdd = isQuickAdd;
                shelf.IsPublic = isPublic;
                shelf.TrackUpdates = trackUpdates;
                shelf.Color = color;
                shelf.IconId = icon;

                await _context.SaveChangesAsync(cancellationToken);

                return new CreatedAtActionResult(
                    nameof(ShelvesController.GetShelf),
                    nameof(ShelvesController)[..^10],
                    new { shelf.Id },
                    shelf
                );
            }
        }
    }
}