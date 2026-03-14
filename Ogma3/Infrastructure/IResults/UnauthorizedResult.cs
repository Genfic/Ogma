using System.Reflection;
using Microsoft.AspNetCore.Http.Metadata;

namespace Ogma3.Infrastructure.IResults;

public sealed class UnauthorizedResult : IResult, IEndpointMetadataProvider
{
	public Task ExecuteAsync(HttpContext httpContext)
	{
		ArgumentNullException.ThrowIfNull(httpContext);

		var loggerFactory = httpContext.RequestServices.GetRequiredService<ILoggerFactory>();
		var logger = loggerFactory.CreateLogger("Ogma3.Infrastructure.IResults.UnauthorizedResult");
		Log.LogStatusMessage(logger, StatusCodes.Status401Unauthorized);

		httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;

		return Task.CompletedTask;
	}

	public static void PopulateMetadata(MethodInfo method, EndpointBuilder builder)
	{
		builder.Metadata.Add(new ProducesResponseTypeMetadata(
			StatusCodes.Status401Unauthorized,
			typeof(void),
			["application/json"]
		));
	}
}

public static partial class Log
{
	[LoggerMessage(1, LogLevel.Information, "Setting HTTP status code {StatusCode}.", EventName = "WritingResultAsStatusCode")]
	public static partial void LogStatusMessage(ILogger logger, int statusCode);
}