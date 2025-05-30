using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;
using Ogma3.Infrastructure.Sqids;

namespace Ogma3.Infrastructure.OpenApi.Transformers;

public sealed class SqidSchemaTransformer : IOpenApiSchemaTransformer
{
	public Task TransformAsync(OpenApiSchema schema, OpenApiSchemaTransformerContext context, CancellationToken cancellationToken)
	{
		if (context.JsonTypeInfo.Type != typeof(Sqid))
		{
			return Task.CompletedTask;
		}

		schema.Type = "string";
		schema.Format = "sqid";
		schema.Description = "SQID-encoded value";
		schema.Reference = null;
		schema.Properties.Clear();
		schema.Required.Clear();

		return Task.CompletedTask;
	}
}