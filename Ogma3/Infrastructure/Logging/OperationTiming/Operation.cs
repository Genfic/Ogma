using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Ogma3.Infrastructure.Logging.OperationTiming;

[SuppressMessage(
	"Usage",
	"CA2254:Template should be a static expression",
	Justification = "Extends an existing structured message template.")]
public sealed class Operation(ILogger logger, LogLevel level, string messageTemplate, params object[] args) : IDisposable
{
	private readonly long _start = Stopwatch.GetTimestamp();

	public void Dispose()
	{
		var elapsed = Stopwatch.GetElapsedTime(_start).TotalMilliseconds;
		var template = $"{messageTemplate} completed in {{Elapsed:0.0}} ms";
		object[] newArgs = [..args, elapsed];
		logger.Log(level, template, newArgs);
	}
}