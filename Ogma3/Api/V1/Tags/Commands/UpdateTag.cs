#nullable enable
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Tags;

namespace Ogma3.Api.V1.Tags.Commands
{
    public static class UpdateTag
    {
        public sealed record Query(long Id, string? Name, string? Description, ETagNamespace? Namespace) : IRequest<IActionResult>;

        public class Handler : IRequestHandler<Query, IActionResult>
        {
            private readonly ApplicationDbContext _context;
            public Handler(ApplicationDbContext context) => _context = context;

            public async Task<IActionResult> Handle(Query request, CancellationToken cancellationToken)
            {
                var (id, name, description, ns) = request;
                
                // Different ID but the same (name, namespace) tuple means update would make the tag a duplicate
                var duplicateExists = await _context.Tags
                    .Where(t => t.Id != id)
                    .Where(t => t.Name == name && t.Namespace == ns)
                    .AnyAsync(cancellationToken);

                if (duplicateExists) return new ConflictObjectResult($"Tag {name} already exists in the {ns} namespace.");

                var tag = await _context.Tags
                    .Where(t => t.Id == id)
                    .FirstOrDefaultAsync(cancellationToken);

                if (tag is null) return new NotFoundResult();

                tag.Name = name ?? tag.Name;
                tag.Description = description;
                tag.Namespace = ns;
                
                await _context.SaveChangesAsync(cancellationToken);

                return new OkObjectResult(tag);
            }
        }
    }
}