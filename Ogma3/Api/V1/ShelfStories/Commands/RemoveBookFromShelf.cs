using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.ShelfStories.Commands;

public static class RemoveBookFromShelf
{
    public sealed record Command(long ShelfId, long StoryId) : IRequest<ActionResult<Result>>;

    public class Handler : IRequestHandler<Command, ActionResult<Result>>
    {
        private readonly ApplicationDbContext _context;
        private readonly long? _uid;

        public Handler(ApplicationDbContext context, IUserService userService)
        {
            _context = context;
            _uid = userService.User?.GetNumericId();
        }

        public async Task<ActionResult<Result>> Handle(Command request, CancellationToken cancellationToken)
        {
            if (_uid is null) return new UnauthorizedResult();

            var (shelfId, storyId) = request;

            var shelfStory = await _context.ShelfStories
                .Where(ss => ss.ShelfId == shelfId)
                .Where(ss => ss.StoryId == storyId)
                .FirstOrDefaultAsync(cancellationToken);

            if (shelfStory is null) return new NotFoundResult();

            _context.ShelfStories.Remove(shelfStory);

            await _context.SaveChangesAsync(cancellationToken);
            return new OkObjectResult(new Result(shelfId, storyId));
        }
    }
        
    public sealed record Result(long ShelfId, long StoryId);
}