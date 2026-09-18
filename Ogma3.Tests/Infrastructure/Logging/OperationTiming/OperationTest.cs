using Microsoft.Extensions.Logging;
using Ogma3.Infrastructure.Logging.OperationTiming;

namespace Ogma3.Tests.Infrastructure.Logging.OperationTiming;

public sealed class OperationTest
{
	private sealed class RecorderLogger : ILogger
	{
		public LogLevel? RecordedLevel { get; private set; }

		public string? RecordedMessage { get; private set; }

		public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

		public bool IsEnabled(LogLevel logLevel) => true;

		public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
		{
			RecordedLevel = logLevel;
			RecordedMessage = formatter(state, exception);
		}
	}

	[Test]
	public async Task TestTimeOperationDefaultsToInformation()
	{
		var logger = new RecorderLogger();
		using (logger.TimeOperation("Doing work"))
		{
		}

		await Assert.That(logger.RecordedLevel).IsEqualTo(LogLevel.Information);
		await Assert.That(logger.RecordedMessage).Contains("Doing work completed in");
		await Assert.That(logger.RecordedMessage).EndsWith("ms");
	}

	[Test]
	public async Task TestTimeOperationWithExplicitLevel()
	{
		var logger = new RecorderLogger();
		using (logger.TimeOperation(LogLevel.Warning, "Slow thing"))
		{
		}

		await Assert.That(logger.RecordedLevel).IsEqualTo(LogLevel.Warning);
	}

	[Test]
	public async Task TestTimeOperationWithStructuredArgs()
	{
		var logger = new RecorderLogger();
		using (logger.TimeOperation("Processing {Id}", 42))
		{
		}

		await Assert.That(logger.RecordedMessage).Contains("42");
		await Assert.That(logger.RecordedMessage).EndsWith("ms");
	}
}