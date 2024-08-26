using System.Reflection;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http.Metadata;

namespace Ogma3.Infrastructure.IResults;

public partial class ServerError : IResult, IEndpointMetadataProvider, IStatusCodeHttpResult
{
	internal ServerError()
	{}
	
	public static ServerError Instance() => new();

	[UsedImplicitly]
	public int StatusCode => StatusCodes.Status500InternalServerError;
	
	int? IStatusCodeHttpResult.StatusCode => StatusCode;

	public Task ExecuteAsync(HttpContext httpContext)
	{
		ArgumentNullException.ThrowIfNull(httpContext);

		// Creating the logger with a string to preserve the category after the refactoring.
		var loggerFactory = httpContext.RequestServices.GetRequiredService<ILoggerFactory>();
		var logger = loggerFactory.CreateLogger($"Ogma3.Infrastructure.IResults.{nameof(ServerError)}");

		WriteResultAsStatusCode(logger, StatusCode);
		httpContext.Response.StatusCode = StatusCode;

		return Task.CompletedTask;
	}

	/// <inheritdoc/>
	static void IEndpointMetadataProvider.PopulateMetadata(MethodInfo method, EndpointBuilder builder)
	{
		ArgumentNullException.ThrowIfNull(method);
		ArgumentNullException.ThrowIfNull(builder);

		builder.Metadata.Add(new ProducesResponseTypeMetadata(StatusCodes.Status500InternalServerError, typeof(void)));
	}
	
	[LoggerMessage(1, LogLevel.Information,
		"Setting HTTP status code {StatusCode}.",
		EventName = "WritingResultAsStatusCode")]
	private static partial void WriteResultAsStatusCode(ILogger logger, int statusCode);
}