using Microsoft.OpenApi;

namespace Ogma3.Infrastructure.Extensions;

public static class EndpointExtensions
{
	extension<TBuilder>(TBuilder builder) where TBuilder : IEndpointConventionBuilder
	{
		public TBuilder WithHeader(
			string responseCode,
			string name,
			string description
		)
		{
			return builder.AddOpenApiOperationTransformer((operation, _, _) => {
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
		public TBuilder ConfigureIf(bool condition, Func<TBuilder, TBuilder> config)
			=> condition
				? config(builder)
				: builder;

		public TBuilder ConfigureIf(Func<bool> condition, Func<TBuilder, TBuilder> config)
			=> builder.ConfigureIf(condition(), config);
	}

}