using JetBrains.Annotations;

namespace Ogma3.Infrastructure.Logging.OperationTiming;

public static class LoggerExtension
{
	public static Operation TimeOperation(this ILogger logger, [StructuredMessageTemplate] string messageTemplate, params object[] args)
	{
		return new Operation(logger, LogLevel.Information, messageTemplate, args);
	}
	
	public static Operation TimeOperation(this ILogger logger, LogLevel level, [StructuredMessageTemplate] string messageTemplate, params object[] args)
	{
		return new Operation(logger, level, messageTemplate, args);
	}
}