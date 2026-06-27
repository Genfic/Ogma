using JetBrains.Annotations;

namespace Ogma3.Infrastructure.Logging.OperationTiming;

public static class LoggerExtension
{
	extension(ILogger logger)
	{
		public Operation TimeOperation([StructuredMessageTemplate] string messageTemplate, params object[] args)
		{
			return new Operation(logger, LogLevel.Information, messageTemplate, args);
		}

		public Operation TimeOperation(LogLevel level, [StructuredMessageTemplate] string messageTemplate, params object[] args)
		{
			return new Operation(logger, level, messageTemplate, args);
		}
	}
}