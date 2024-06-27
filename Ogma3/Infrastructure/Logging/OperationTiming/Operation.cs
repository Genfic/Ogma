using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;

namespace Ogma3.Infrastructure.Logging.OperationTiming;

[SuppressMessage("Usage", "CA2254")]
public sealed class Operation(ILogger logger, LogLevel level, string messageTemplate, params object[] args) : IDisposable
{
	private static readonly double StopwatchToTimeSpanTicks = Stopwatch.Frequency / 10_000_000.0;
	
	private readonly long _start = (long)(Stopwatch.GetTimestamp() / StopwatchToTimeSpanTicks);

	public void Dispose()
	{
		var elapsed = Stopwatch.GetElapsedTime(_start).TotalMilliseconds;
		var template = $"{messageTemplate} in {{Elapsed:0.0}}ms";
		object[] newArgs = [..args, elapsed];
		logger.Log(level, template, newArgs);
	}
}