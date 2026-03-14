using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Ogma3.Infrastructure.OpenApi.Transformers;

public sealed class UnauthorizedResultOperationTransformer : IOpenApiOperationTransformer
{
	public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
	{
		var hasAuth = context.Description.ActionDescriptor.EndpointMetadata
			.OfType<IAuthorizeData>()
			.Any();

		if (!hasAuth)
		{
			return Task.CompletedTask;
		}

		operation.Responses ??= new OpenApiResponses();
		operation.Responses.TryAdd("401", new OpenApiResponse { Description = "Unauthorized" });

		return Task.CompletedTask;
	}
}