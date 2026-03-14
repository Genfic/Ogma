using System.Reflection;
using Microsoft.AspNetCore.Http.Metadata;

namespace Ogma3.Infrastructure.IResults;

public sealed class NotModifiedResult : IResult, IEndpointMetadataProvider
{
	public Task ExecuteAsync(HttpContext httpContext)
	{
		httpContext.Response.StatusCode = StatusCodes.Status304NotModified;
		return Task.CompletedTask;
	}

	public static void PopulateMetadata(MethodInfo method, EndpointBuilder builder)
	{
		builder.Metadata.Add(new ProducesResponseTypeMetadata(
			StatusCodes.Status304NotModified,
			typeof(void),
			["application/json"]
		));
	}
}