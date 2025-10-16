using Microsoft.OpenApi;

namespace Ogma3.Infrastructure.Extensions;

public static class EndpointExtensions
{
	public static IEndpointConventionBuilder WithHeader(
		this IEndpointConventionBuilder builder,
		string responseCode,
		string name,
		string description
	)
	{
		return builder.AddOpenApiOperationTransformer((operation, ctx, ct) => {
			var response = operation.Responses?[responseCode];

			response?.Headers?.Add(name, new OpenApiHeader
			{
				Description = description,
				Schema = new OpenApiSchema
				{
					Type = JsonSchemaType.String,
				},
			});

			return Task.CompletedTask;
		});
	}
}