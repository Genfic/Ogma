using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;

namespace Ogma3.Infrastructure.NSwag.OperationProcessors;

public class ExcludeInternalApisProcessor : IOperationProcessor
{
    public bool Process(OperationProcessorContext context) 
        => !(context.ControllerType.FullName?.ToUpper().Contains("ADMIN") ?? false);
}