using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;

namespace Ogma3.Areas.Admin.Api.V1.Telemetry.Queries;

public static class GetImportantItemCounts
{
    public sealed record Query : IRequest<ActionResult<Dictionary<string, int>>>;

    public class Handler : IRequestHandler<Query, ActionResult<Dictionary<string, int>>>
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<ActionResult<Dictionary<string, int>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var counts = await _context.TableRowCounts.FromSqlRaw(@"
                    SELECT 'Stories' as name, count(1) as count FROM ""Stories"" UNION
                    SELECT 'Chapters' as name, count(1) as count FROM ""Chapters"" UNION
                    SELECT 'Blogposts' as name, count(1) as count FROM ""Blogposts"" UNION
                    SELECT 'Users' as name, count(1) as count FROM ""AspNetUsers"" u WHERE u.""EmailConfirmed"" UNION
                    SELECT 'Comments' as name, count(1) as count FROM ""Comments"" c WHERE c.""DeletedBy"" is null UNION
                    SELECT 'Reports' as name, count(1) as count FROM ""Reports"";
                ")
                .ToDictionaryAsync(x => x.Name, x => x.Count, cancellationToken);

            return new OkObjectResult(counts);
        }
    }
}