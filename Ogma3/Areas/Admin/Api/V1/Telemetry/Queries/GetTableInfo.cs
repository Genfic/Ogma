using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Areas.Admin.Models;
using Ogma3.Data;

namespace Ogma3.Areas.Admin.Api.V1.Telemetry.Queries;

public static class GetTableInfo
{
	public sealed record Query : IRequest<ActionResult<List<TableInfo>>>;

	public class Handler : IRequestHandler<Query, ActionResult<List<TableInfo>>>
	{
		private readonly ApplicationDbContext _context;
		public Handler(ApplicationDbContext context) => _context = context;

		public async Task<ActionResult<List<TableInfo>>> Handle(Query request, CancellationToken cancellationToken)
		{
			var info = await _context.TableInfos.FromSqlRaw(@"
                SELECT 
                    table_name as name, 
                    pg_relation_size(quote_ident(table_name)) as size
                FROM information_schema.tables 
                WHERE table_schema = 'public' AND table_name NOT LIKE '\_\_%'
                ORDER BY size DESC
            ").ToListAsync(cancellationToken);

			return new OkObjectResult(info);
		}
	}
}