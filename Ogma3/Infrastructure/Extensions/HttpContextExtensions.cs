using Ogma3.Infrastructure.Attributes;

namespace Ogma3.Infrastructure.Extensions;

public static class HttpContextExtensions
{
	extension(HttpContext ctx)
	{
		public bool IsApiEndpoint()
		{
			var endpoint = ctx.GetEndpoint();
			var isApiEndpoint = endpoint?.Metadata.GetMetadata<ApiEndpointAttribute>() is not null;
			return isApiEndpoint;
		}
	}
}