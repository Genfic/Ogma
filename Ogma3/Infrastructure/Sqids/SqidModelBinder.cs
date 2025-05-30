using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Ogma3.Infrastructure.Sqids;

public sealed class SqidModelBinder : IModelBinder
{
	public Task BindModelAsync(ModelBindingContext context)
	{
		var value = context.ValueProvider.GetValue(context.ModelName).FirstValue;
		if (Sqid.TryParse(value, null, out var sqid))
		{
			context.Result = ModelBindingResult.Success(sqid);
		}
		else
		{
			context.ModelState.AddModelError(context.ModelName, "Invalid SQID.");
		}
		return Task.CompletedTask;
	}

	public sealed class Provider : IModelBinderProvider
	{
		public IModelBinder? GetBinder(ModelBinderProviderContext context)
			=> context.Metadata.ModelType == typeof(Sqid) ? new SqidModelBinder() : null;
	}
}