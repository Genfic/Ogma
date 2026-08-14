using System.Text.Json;
using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Infrastructure.OgmaConfig;

namespace Ogma3.Api.V1;

[Handler]
[MapGroup<ApiGroup>]
[MapGet("test-three")]
public sealed partial class TestThree(OgmaConfig config, AppDbContext ctx)
{
	JsonSerializerOptions opt = new() { WriteIndented = true };

	private async ValueTask<Ok<string>> Handle(Query q, CancellationToken ct)
	{
		var ns = await ctx.TagNamespaces.ToListAsync(ct);
		var t = await ctx.Tags.Select(t => new
			{
				t.Name,
				t.NamespaceId,
				NsName = t.Namespace == null ? null : t.Namespace.Name,
				NsCol = t.Namespace == null ? null : t.Namespace.Color,
			})
			.ToListAsync(ct);

		var json =  JsonSerializer.Serialize(new { ns, t }, opt);
		return TypedResults.Ok(json);
	}

	public sealed record Query;
}