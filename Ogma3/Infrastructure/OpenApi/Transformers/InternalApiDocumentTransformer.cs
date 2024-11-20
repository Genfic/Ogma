using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;
using NuGet.Packaging;

namespace Ogma3.Infrastructure.OpenApi.Transformers;

public sealed class InternalApiDocumentTransformer : IOpenApiDocumentTransformer
{
	public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
	{
		var paths = document.Paths
			.Where(p => p.Key.Trim('/').StartsWith("admin"))
			.ToDictionary(kv => kv.Key, kv => kv.Value);
		
		document.Paths.Clear();
		document.Paths.AddRange(paths);
		
		return Task.CompletedTask;	
	}
}