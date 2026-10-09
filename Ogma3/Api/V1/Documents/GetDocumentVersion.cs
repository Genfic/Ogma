using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;

namespace Ogma3.Api.V1.Documents;

[Handler]
[MapGroup<ApiGroup>]
[MapGet("documents/{slug}/version/{version}")]
[UsedImplicitly]
public sealed partial class GetDocumentVersion(AppDbContext context)
{
	private static void CustomizeEndpoint(RouteHandlerBuilder endpoint)
		=> endpoint.AddEndpointFilter(async (ctx, next) => {
			ctx.HttpContext.Response.Headers.CacheControl = "public, max-age=31536000, immutable";
			return await next(ctx);
		});

	private async ValueTask<Results<NotFound, Ok<Result>>> HandleAsync(Query request, CancellationToken cancellationToken)
	{
		var document = await context.Documents
			.Where(d => d.Slug == request.Slug && d.Version == request.Version)
			.Select(d => new Result(d.Version, d.CompiledBody))
			.FirstOrDefaultAsync(cancellationToken);

		return document is null
			? TypedResults.NotFound()
			: TypedResults.Ok(document);
	}

	[UsedImplicitly]
	public sealed record Query(string Slug, uint Version);

	public sealed record Result(uint Version, string CompiledBody);
}
