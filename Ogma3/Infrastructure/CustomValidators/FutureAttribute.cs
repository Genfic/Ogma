using Immediate.Validations.Shared;

namespace Ogma3.Infrastructure.CustomValidators;

public sealed class FutureAttribute : ValidatorAttribute
{
	public static bool ValidateProperty(DateTimeOffset value)
	{
		return value > DateTimeOffset.UtcNow;
	}

	public static string DefaultMessage => "{PropertyName} must be in the future.";
}