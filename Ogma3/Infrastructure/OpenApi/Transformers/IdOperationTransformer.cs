using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;
using Utils.Extensions;

namespace Ogma3.Infrastructure.OpenApi.Transformers;

public sealed class IdOperationTransformer : IOpenApiOperationTransformer
{
	public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
	{
		if (!string.IsNullOrEmpty(operation.OperationId))
		{
			return Task.CompletedTask;
		}

		if (context.Description.HttpMethod is null || context.Description.RelativePath is null)
		{
			return Task.CompletedTask;
		}

		var method = context.Description.HttpMethod.ToLower().Capitalize();
		var description = context.Description.RelativePath.Split('/', '-', '_', '.')
			.Where(p => p is not ['{', .., '}'])
			.Select(p => p.Capitalize());

		operation.OperationId = $"{method}{string.Join("", description)}";
		
		return Task.CompletedTask;
	}
}