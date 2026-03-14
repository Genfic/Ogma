using Microsoft.AspNetCore.OpenApi;

namespace Ogma3.Infrastructure.OpenApi.Transformers;

public static class SharedTransformersExtensions
{
		extension(OpenApiOptions options)
		{
			public OpenApiOptions AddSharedOperationTransformers()
			{
				options.AddOperationTransformer<MinimalApiTagOperationTransformer>();
				options.AddOperationTransformer<IdOperationTransformer>();
				options.AddOperationTransformer<UnauthorizedResultOperationTransformer>();
				options.AddOperationTransformer<TooManyRequestsOperationTransformer>();
				return options;
			}
		}

}