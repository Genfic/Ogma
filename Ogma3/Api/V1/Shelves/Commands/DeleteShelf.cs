using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.Shelves.Commands
{
    public static class DeleteShelf
    {
        public sealed record Command(long ShelfId) : IRequest<ActionResult<long>>;

        public class Handler : IRequestHandler<Command, ActionResult<long>>
        {
            private readonly ApplicationDbContext _context;
            private readonly long? _uid;
            public Handler(ApplicationDbContext context, IUserService userService)
            {
                _context = context;
                _uid = userService.User?.GetNumericId();
            }

            public async Task<ActionResult<long>> Handle(Command request, CancellationToken cancellationToken)
            {
                if (_uid is null) return new UnauthorizedResult();

                var shelf = await _context.Shelves
                    .Where(s => s.Id == request.ShelfId)
                    .Where(s => s.OwnerId == _uid)
                    .FirstOrDefaultAsync(cancellationToken);

                if (shelf is null) return new NotFoundResult();
                if (shelf.OwnerId != _uid) return new UnauthorizedResult();

                _context.Shelves.Remove(shelf);
                await _context.SaveChangesAsync(cancellationToken);

                return new OkObjectResult(shelf.Id);
            }
        }
    }
}