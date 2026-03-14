using Microsoft.AspNetCore.OpenApi;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.OpenApi;

namespace Ogma3.Infrastructure.OpenApi.Transformers;

public sealed class TooManyRequestsOperationTransformer : IOpenApiOperationTransformer
{
	public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
	{
		var hasRateLimit = context.Description.ActionDescriptor.EndpointMetadata
			.OfType<EnableRateLimitingAttribute>()
			.Any();

		if (!hasRateLimit)
		{
			return Task.CompletedTask;
		}

		operation.Responses ??= new OpenApiResponses();
		operation.Responses.TryAdd("429", new OpenApiResponse { Description = "Too Many Requests" });

		return Task.CompletedTask;
	}
}