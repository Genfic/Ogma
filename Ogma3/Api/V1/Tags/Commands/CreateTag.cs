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
    public static class CreateTag
    {
        public sealed record Query(string Name, string? Description, ETagNamespace? Namespace) : IRequest<IActionResult>;

        public class Handler : IRequestHandler<Query, IActionResult>
        {
            private readonly ApplicationDbContext _context;
            public Handler(ApplicationDbContext context) => _context = context;

            public async Task<IActionResult> Handle(Query request, CancellationToken cancellationToken)
            {
                var (name, description, ns) = request;
                
                var tagExist = await _context.Tags
                    .Where(t => t.Name == name && t.Namespace == ns)
                    .AnyAsync(cancellationToken);

                if (tagExist) return new ConflictObjectResult($"Tag {name} already exists in the {ns} namespace.");

                var entity = _context.Tags.Add(new Tag
                {
                    Name = name,
                    Description = description,
                    Namespace = ns
                });
                await _context.SaveChangesAsync(cancellationToken);

                return new CreatedAtActionResult(
                    nameof(TagsController.GetTag),
                    nameof(TagsController)[..^10],
                    new { entity.Entity.Id }, 
                    entity.Entity
                );
            }
        }
    }
}