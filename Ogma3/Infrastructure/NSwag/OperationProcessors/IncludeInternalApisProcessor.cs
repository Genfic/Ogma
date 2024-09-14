using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;

namespace Ogma3.Infrastructure.NSwag.OperationProcessors;

public sealed class IncludeInternalApisProcessor : IOperationProcessor
{
	public bool Process(OperationProcessorContext context)
	{
		return context.OperationDescription.Path.StartsWith("/ADMIN", StringComparison.CurrentCultureIgnoreCase);
	}
}