using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Infrastructure.MediatR.Bases;

namespace Ogma3.Api.V1.Tags.Commands;

public static class DeleteTag
{
    public sealed record Command(long Id) : IRequest<ActionResult<long>>;

    public class Handler : BaseHandler, IRequestHandler<Command, ActionResult<long>>
    {
        private readonly ApplicationDbContext _context;

        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<ActionResult<long>> Handle(Command request, CancellationToken cancellationToken)
        {
            var tag = await _context.Tags
                .Where(t => t.Id == request.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (tag is null) return NotFound();

            _context.Tags.Remove(tag);
            await _context.SaveChangesAsync(cancellationToken);

            return Ok(tag.Id);
        }
    }
}