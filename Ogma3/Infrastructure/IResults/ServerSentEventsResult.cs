namespace Ogma3.Infrastructure.IResults;

public sealed class ServerSentEventsResult(
	TimeSpan interval,
	Func<CancellationToken, ValueTask<string>> produce,
	CancellationToken cancellationToken
) : IResult
{
	public async Task ExecuteAsync(HttpContext httpContext)
	{
		var response = httpContext.Response;
		response.ContentType = "text/event-stream";
		response.Headers.CacheControl = "no-cache";
		response.Headers["X-Accel-Buffering"] = "no";

		using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, httpContext.RequestAborted);
		var token = cts.Token;

		try
		{
			using var timer = new PeriodicTimer(interval);

			do
			{
				var payload = await produce(token);
				await response.WriteAsync($"data: {payload}\n\n", token);
				await response.Body.FlushAsync(token);
			}
			while (await timer.WaitForNextTickAsync(token));
		}
		catch (OperationCanceledException) when (token.IsCancellationRequested)
		{
			// client went away
		}
	}
}
