using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace Ogma3.Infrastructure.OpenApi.Transformers;

public sealed class MinimalApiTagOperationTransformer : IOpenApiOperationTransformer
{
	public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
	{
		if (context.Description.RelativePath?.Split("/", StringSplitOptions.RemoveEmptyEntries) is not {} split)
		{
			return Task.CompletedTask;
		}
		
		split = split[0] == "admin" ? split[1..] : split;
		
		if (split is not ["api", var name, ..])
		{
			return Task.CompletedTask;
		}
		
		operation.Tags.Clear();
		operation.Tags.Add(new OpenApiTag
		{
			Name = name,
		});
		
		return Task.CompletedTask;
	}
}