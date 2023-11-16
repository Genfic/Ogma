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

	public sealed record Response(string Name, int Count);

	public class Handler(ApplicationDbContext context) : IRequestHandler<Query, ActionResult<Dictionary<string, int>>>
	{
		public async Task<ActionResult<Dictionary<string, int>>> Handle(Query request, CancellationToken cancellationToken)
		{
			var counts = await context.Database.SqlQueryRaw<Response>("""
			    	SELECT 'Stories' as name, count(1) as count FROM "Stories" UNION
			    	SELECT 'Chapters' as name, count(1) as count FROM "Chapters" UNION
			    	SELECT 'Blogposts' as name, count(1) as count FROM "Blogposts" UNION
			    	SELECT 'Users' as name, count(1) as count FROM "AspNetUsers" u WHERE u."EmailConfirmed" UNION
			    	SELECT 'Comments' as name, count(1) as count FROM "Comments" c WHERE c."DeletedBy" is null UNION
			    	SELECT 'Reports' as name, count(1) as count FROM "Reports";
			    """)
				.ToDictionaryAsync(i => i.Name, i => i.Count, cancellationToken);

			return new OkObjectResult(counts);
		}
	}
}