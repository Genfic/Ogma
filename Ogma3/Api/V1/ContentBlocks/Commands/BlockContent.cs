using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ogma3.Data.Bases;
using Ogma3.Data.Blacklists;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.ContentBlocks.Commands
{
    public static class BlockContent
    {
        public sealed record Command<T>(long ObjectId, string Reason) : IRequest<ActionResult> where T : BaseModel, IBlockableContent;

        public class Handler<T> : IRequestHandler<Command<T>, ActionResult> where T : BaseModel, IBlockableContent
        {
            private readonly ApplicationDbContext _context;
            private readonly long? _uid;
            public Handler(ApplicationDbContext context, IUserService userService)
            {
                _context = context;
                _uid = userService.User?.GetNumericId();
            }

            public async Task<ActionResult> Handle(Command<T> request, CancellationToken cancellationToken)
            {
                var (itemId, reason) = request;

                if (_uid is null) return new UnauthorizedResult();
            
                var item = await _context.Set<T>()
                    .Where(i => i.Id == itemId)
                    .FirstOrDefaultAsync(cancellationToken);
                if (item is null) return new NotFoundResult();

                item.ContentBlock = new ContentBlock
                {
                    Reason = reason,
                    IssuerId = (long) _uid
                };
                await _context.SaveChangesAsync(cancellationToken);

                return new OkResult();
            }
        }
    }
}