using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Infrastructure.MediatR.Bases;

namespace Ogma3.Api.V1.Faqs.Commands;

public static class DeleteFaq
{
    public sealed record Command(long Id) : IRequest<ActionResult<long>>;

    public class Handler : BaseHandler, IRequestHandler<Command, ActionResult<long>>
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<ActionResult<long>> Handle(Command request, CancellationToken cancellationToken)
        {
            var faq = await _context.Faqs
                .Where(f => f.Id == request.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (faq is null) return NotFound();

            _context.Remove(faq);

            await _context.SaveChangesAsync(cancellationToken);

            return Ok(faq.Id);
        }
    }
}