using Microsoft.OpenApi.Models;

namespace Ogma3.Infrastructure.Extensions;

public static class EndpointExtensions
{
	public static IEndpointConventionBuilder WithHeader(this IEndpointConventionBuilder builder, string responseCode, string name, string description)
	{
		return builder.WithOpenApi(operation => {
			if (!operation.Responses.TryGetValue(responseCode, out var response))
			{
				response = new OpenApiResponse { Description = "Default response" };
				operation.Responses[responseCode] = response;
			}

			response.Headers ??= new Dictionary<string, OpenApiHeader>();
			response.Headers[name] = new OpenApiHeader
			{
				Description = description,
				Schema = new OpenApiSchema
				{
					Type = "string",
					Nullable = true,
				},
			};

			return operation;
		});
	}
}