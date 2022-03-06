using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ogma3.Data;
using Ogma3.Data.Shelves;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Infrastructure.MediatR.Bases;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.Shelves.Commands;

public static class CreateShelf
{
    public sealed record Command(
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
            RuleFor(s => s.Name)
                .MaximumLength(CTConfig.CShelf.MaxNameLength)
                .MinimumLength(CTConfig.CShelf.MinNameLength);
            RuleFor(s => s.Description)
                .MaximumLength(CTConfig.CShelf.MaxDescriptionLength);
            RuleFor(s => s.Color)
                .Length(7);
        }
    }

    public class Handler : BaseHandler, IRequestHandler<Command, ActionResult<ShelfDto>>
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
            if (_uid is null) return Unauthorized();
                
            var (name, description, isQuickAdd, isPublic, trackUpdates, color, icon) = request;
            var shelf = new Shelf
            {
                Name = name,
                Description = description,
                IsQuickAdd = isQuickAdd,
                IsPublic = isPublic,
                TrackUpdates = trackUpdates,
                Color = color,
                IconId = icon,
                OwnerId = (long)_uid,
            };

            _context.Add(shelf);
            await _context.SaveChangesAsync(cancellationToken);

            return CreatedAtAction(
                nameof(ShelvesController.GetShelf),
                nameof(ShelvesController)[..^10],
                new { shelf.Id },
                shelf
            );
        }
    }
}