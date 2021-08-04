using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Shelves;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.Shelves.Queries
{
    public static class GetShelf
    {
        public sealed record Query(long ShelfId) : IRequest<ActionResult<ShelfDto>>;

        public class Handler : IRequestHandler<Query, ActionResult<ShelfDto>>
        {
            private readonly ApplicationDbContext _context;
            private readonly IMapper _mapper;
            private readonly long? _uid;
            
            public Handler(ApplicationDbContext context, IMapper mapper, IUserService userService)
            {
                _context = context;
                _mapper = mapper;
                _uid = userService.User?.GetNumericId();
            }

            public async Task<ActionResult<ShelfDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                if (_uid is null) return new UnauthorizedResult();
                
                var shelf = await _context.Shelves
                    .Where(s => s.Id == request.ShelfId)
                    .Where(s => s.IsPublic || s.OwnerId == _uid)
                    .ProjectTo<ShelfDto>(_mapper.ConfigurationProvider)
                    .FirstOrDefaultAsync(cancellationToken);

                return shelf is null 
                    ? new NotFoundResult() 
                    : new OkObjectResult(shelf);
            }
        }
    }
}