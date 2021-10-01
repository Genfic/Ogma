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

namespace Ogma3.Api.V1.ChaptersReads.Queries;

public static class GetReadChapters
{
    public sealed record Query(long Id) : IRequest<ActionResult<ICollection<long>>>;
        
    public class Handler : IRequestHandler<Query, ActionResult<ICollection<long>>>
    {
        private readonly ApplicationDbContext _context;
        private readonly long? _uid;

        public Handler(ApplicationDbContext context, IUserService userService)
        {
            _context = context;
            _uid = userService?.User?.GetNumericId();
        }
            
        public async Task<ActionResult<ICollection<long>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var chaptersRead = await _context.ChaptersRead
                .Where(cr => cr.StoryId == request.Id)
                .Where(cr => cr.UserId == _uid)
                .Select(cr => cr.Chapters)
                .FirstOrDefaultAsync(cancellationToken);

            return new OkObjectResult(chaptersRead ?? new HashSet<long>());
        }
    }
        
}