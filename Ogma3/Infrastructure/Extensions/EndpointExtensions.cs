using Microsoft.OpenApi;

namespace Ogma3.Infrastructure.Extensions;

public static class EndpointExtensions
{
	public static IEndpointConventionBuilder WithHeader(this IEndpointConventionBuilder builder, string responseCode, string name, string description)
	{
		return builder.AddOpenApiOperationTransformer((operation, ctx, ct) => {

			IOpenApiResponse? response = null;
			if (operation.Responses?.TryGetValue(responseCode, out response) is true)
			{
				response = new OpenApiResponse { Description = "Default response" };
				operation.Responses[responseCode] = response;
			}

			response?.Headers?.Clear();
			response?.Headers?[name] = new OpenApiHeader
			{
				Description = description,
				Schema = new OpenApiSchema
				{
					Type = JsonSchemaType.String,
				},
			};

			return Task.FromResult(operation);
		});
	}
}