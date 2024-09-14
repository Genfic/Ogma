using Humanizer;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;

namespace Ogma3.Infrastructure.NSwag.OperationProcessors;

public sealed class MinimalApiTagProcessor : IOperationProcessor
{
	public bool Process(OperationProcessorContext context)
	{
		if (context.ControllerType is not null)
		{
			return true;
		}
		
		if (context.OperationDescription.Operation.Tags is { Count: > 0 })
		{
			return true;
		}
		
		var path = context.OperationDescription.Path;
		var tag = path
			.Replace("api", "", StringComparison.InvariantCultureIgnoreCase)
			.Split('/', StringSplitOptions.RemoveEmptyEntries)[0];
		context.OperationDescription.Operation.Tags.Add(tag.Pascalize());
		
		return true;
	}
}