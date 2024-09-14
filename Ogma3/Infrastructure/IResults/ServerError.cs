using System.Reflection;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http.Metadata;

namespace Ogma3.Infrastructure.IResults;

public sealed partial class ServerError : IResult, IEndpointMetadataProvider, IStatusCodeHttpResult
{
	internal ServerError()
	{}
	
	internal ServerError(string message)
	{
		_message = message;
	}
	
	private readonly string? _message;
	
	public static ServerError Instance() => new();
	public static ServerError Instance(string message) => new(message);

	[UsedImplicitly]
	public int StatusCode => StatusCodes.Status500InternalServerError;
	
	int? IStatusCodeHttpResult.StatusCode => StatusCode;

	public async Task ExecuteAsync(HttpContext httpContext)
	{
		ArgumentNullException.ThrowIfNull(httpContext);

		// Creating the logger with a string to preserve the category after the refactoring.
		var loggerFactory = httpContext.RequestServices.GetRequiredService<ILoggerFactory>();
		var logger = loggerFactory.CreateLogger($"Ogma3.Infrastructure.IResults.{nameof(ServerError)}");

		WriteResultAsStatusCode(logger, StatusCode);
		httpContext.Response.StatusCode = StatusCode;

		if (_message is not null)
		{
			await httpContext.Response.WriteAsync(_message);
		}
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