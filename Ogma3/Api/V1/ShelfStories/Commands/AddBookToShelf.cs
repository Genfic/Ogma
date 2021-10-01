using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Shelves;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.ShelfStories.Commands;

public static class AddBookToShelf
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

            var shelfExists = await _context.Shelves
                .Where(s => s.Id == shelfId)
                .AnyAsync(cancellationToken);
            if (!shelfExists) return new NotFoundResult();

            var storyExists = await _context.Stories
                .Where(s => s.Id == storyId)
                .AnyAsync(cancellationToken);
            if (!storyExists) return new NotFoundResult();
                
            _context.ShelfStories.Add(new ShelfStory
            {
                StoryId = storyId,
                ShelfId = shelfId
            });

            await _context.SaveChangesAsync(cancellationToken);
            return new OkObjectResult(new Result(shelfId, storyId));
        }
    }
        
    public sealed record Result(long ShelfId, long StoryId);
}