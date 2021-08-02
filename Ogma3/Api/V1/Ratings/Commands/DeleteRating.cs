using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using B2Net;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;

namespace Ogma3.Api.V1.Ratings.Commands
{
    public static class DeleteRating
    {
        public sealed record Command(long RatingId) : IRequest<ActionResult<long>>;

        public class Handler : IRequestHandler<Command, ActionResult<long>>
        {
            private readonly ApplicationDbContext _context;
            private readonly IB2Client _b2Client;

            public Handler(ApplicationDbContext context, IB2Client b2Client)
            {
                _context = context;
                _b2Client = b2Client;
            }

            public async Task<ActionResult<long>> Handle(Command request, CancellationToken cancellationToken)
            {
                var r = await _context.Ratings
                    .Where(r => r.Id == request.RatingId)
                    .FirstOrDefaultAsync(cancellationToken);

                if (r is null) return new NotFoundResult();

                _context.Ratings.Remove(r);

                if (r.Icon is not null && r.IconId is not null)
                {
                    await _b2Client.Files.Delete(r.IconId, r.Icon, cancellationToken);
                }

                await _context.SaveChangesAsync(cancellationToken);

                return new OkObjectResult(r.Id);
            }
        }
    }
}