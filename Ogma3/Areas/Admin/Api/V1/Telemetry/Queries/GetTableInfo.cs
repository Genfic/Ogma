using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;

namespace Ogma3.Areas.Admin.Api.V1.Telemetry.Queries;

public static class GetTableInfo
{
	public sealed record Query : IRequest<ActionResult<List<Response>>>;

	public sealed record Response(string Name, ulong Size);

	public class Handler(ApplicationDbContext context) : IRequestHandler<Query, ActionResult<List<Response>>>
	{
		public async ValueTask<ActionResult<List<Response>>> Handle(Query request, CancellationToken cancellationToken)
		{
			var info = await context.Database.SqlQueryRaw<Response>("""
				SELECT
				    table_name as name,
				    pg_relation_size(quote_ident(table_name)) as size
				FROM information_schema.tables
				WHERE table_schema = 'public' AND table_name NOT LIKE '\_\_%'
				ORDER BY size DESC
			""").ToListAsync(cancellationToken);

			return new OkObjectResult(info);
		}
	}
}