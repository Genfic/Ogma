using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;

namespace Ogma3.Api.V1.InviteCodes.Commands
{
    public static class DeleteInviteCode
    {
        public sealed record Query(long CodeId) : IRequest<ActionResult<long>>;

        public class Handler : IRequestHandler<Query, ActionResult<long>>
        {
            private readonly ApplicationDbContext _context;
            public Handler(ApplicationDbContext context) => _context = context;

            public async Task<ActionResult<long>> Handle(Query request, CancellationToken cancellationToken)
            {
                var code = await _context.InviteCodes
                    .Where(ic => ic.Id == request.CodeId)
                    .FirstOrDefaultAsync(cancellationToken);

                if (code is null) return new NotFoundResult();

                _context.InviteCodes.Remove(code);

                await _context.SaveChangesAsync(cancellationToken);
                return new OkObjectResult(code.Id);
            }
        }
    }
}