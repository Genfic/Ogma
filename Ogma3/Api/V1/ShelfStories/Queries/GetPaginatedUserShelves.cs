using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.ShelfStories.Queries
{
    public static class GetPaginatedUserShelves
    {
        public sealed record Query(long StoryId, int Page) : IRequest<ActionResult<List<Result>>>;

        public class Handler : IRequestHandler<Query, ActionResult<List<Result>>>
        {
            private readonly ApplicationDbContext _context;
            private readonly OgmaConfig _config;
            private readonly long? _uid;
            
            public Handler(ApplicationDbContext context, IUserService userService, OgmaConfig config)
            {
                _context = context;
                _config = config;
                _uid = userService.User?.GetNumericId();
            }

            public async Task<ActionResult<List<Result>>> Handle(Query request, CancellationToken cancellationToken)
            {
                if (_uid is null) return new UnauthorizedResult();

                var (storyId, page) = request;
                var shelves = await _context.Shelves
                    .Where(s => s.OwnerId == _uid)
                    .Where(s => !s.IsQuickAdd)
                    .Paginate(page, _config.ShelvesPerPage)
                    .Select(s => new Result(
                        s.Id,
                        s.Name,
                        s.Color,
                        s.Icon.Name,
                        s.Stories.Any(x => x.Id == storyId)
                    ))
                    .ToListAsync(cancellationToken);

                return new OkObjectResult(shelves);
            }
        }

        public sealed record Result(long Id, string Name, string Color, string IconName, bool DoesContainBook);
    }
}