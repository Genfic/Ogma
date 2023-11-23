using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;

namespace Ogma3.Infrastructure.NSwag.OperationProcessors;

public class IncludeInternalApisProcessor : IOperationProcessor
{
	public bool Process(OperationProcessorContext context)
		=> context.ControllerType.FullName?.Contains("ADMIN", System.StringComparison.CurrentCultureIgnoreCase) ?? false;
}