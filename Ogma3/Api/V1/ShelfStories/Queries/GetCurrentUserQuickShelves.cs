using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Shelves;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Infrastructure.MediatR.Bases;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.ShelfStories.Queries;

public static class GetCurrentUserQuickShelves
{
    public sealed record Query(long StoryId) : IRequest<ActionResult<List<ShelfDto>>>;

    public class Handler : BaseHandler, IRequestHandler<Query, ActionResult<List<ShelfDto>>>
    {
        private readonly ApplicationDbContext _context;
        private readonly long? _uid;
            
        public Handler(ApplicationDbContext context, IUserService userService)
        {
            _context = context;
            _uid = userService.User?.GetNumericId();
        }

        public async Task<ActionResult<List<ShelfDto>>> Handle(Query request, CancellationToken cancellationToken)
        {
            if (_uid is null) return Unauthorized();
                
            var shelves = await _context.Shelves
                .Where(s => s.OwnerId == _uid)
                .Where(s => s.IsQuickAdd)
                .Select(s => new Result(
                    s.Id,
                    s.Name,
                    s.Color,
                    s.Icon.Name,
                    s.Stories.Any(x => x.Id == request.StoryId)
                ))
                .ToListAsync(cancellationToken);

            return Ok(shelves);
        }
    }
        
    public sealed record Result(long Id, string Name, string Color, string IconName, bool DoesContainBook);
}