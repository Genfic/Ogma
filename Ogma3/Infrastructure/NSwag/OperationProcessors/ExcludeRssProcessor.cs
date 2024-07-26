using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;

namespace Ogma3.Infrastructure.NSwag.OperationProcessors;

public class ExcludeRssProcessor : IOperationProcessor
{
	public bool Process(OperationProcessorContext context)
	{
		return !(context.ControllerType?.FullName?.Contains("rss", StringComparison.InvariantCultureIgnoreCase) ?? false);
	}
}