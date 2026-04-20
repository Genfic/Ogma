using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;

namespace Ogma3.Api.V1;

[Handler]
[MapGet("api/test")]
public sealed partial class TestController(ApplicationDbContext ctx)
{
	public sealed record Query(long Id);

	private async ValueTask<Results<Ok<List<long>>, Ok, NotFound>> HandleAsync(Query q, CancellationToken ct)
	{
		var signature = await ctx.Chapters
			.Where(c => c.Id == q.Id)
			.Select(c => c.Signature)
			.FirstOrDefaultAsync(ct);

		if (signature is null)
		{
			return TypedResults.NotFound();
		}

		var sig = Array.ConvertAll(signature, x => (int)x);
		const double threshold = 0.7;

		var chaps = await ctx.Chapters
			.FromSql($"""
				SELECT * FROM "Chapters" c
				WHERE c."Signature" && {sig} -- initial overlap check
					AND icount(c."Signature" & {sig})::float / {sig.Length} >= {threshold} -- check for exact overlap (threshold)
				ORDER BY icount(c."Signature" & {sig}) DESC
				""")
			.Select(c => c.Id)
			.ToListAsync(ct);

		return TypedResults.Ok(chaps);
	}
}