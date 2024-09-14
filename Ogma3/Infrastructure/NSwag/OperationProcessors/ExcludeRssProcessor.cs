using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;

namespace Ogma3.Infrastructure.NSwag.OperationProcessors;

public sealed class ExcludeRssProcessor : IOperationProcessor
{
	public bool Process(OperationProcessorContext context)
	{
		return !context.OperationDescription.Path.Contains("RSS", StringComparison.CurrentCultureIgnoreCase);
	}
}