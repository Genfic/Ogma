using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Immediate.Validations.Shared;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;

namespace Ogma3.Api.V1;

[Handler]
[MapGet("api/test")]
public static partial class TestController
{
	[Validate]
	public sealed partial record Query(string[] Tags) : IValidationTarget<Query>;

	private static async ValueTask<Ok<List<Bpost>>> HandleAsync(Query q, ApplicationDbContext db, CancellationToken ct)
	{
		var b = await db.Blogposts
			.Where(b => b.Hashtags.Intersect(q.Tags).Any())
			.Select(b => new Bpost(b.Title, b.Hashtags))
			.ToListAsync(ct);
		return TypedResults.Ok(b);
	}

	public record Bpost(string Title, string[] Hashtags);
}