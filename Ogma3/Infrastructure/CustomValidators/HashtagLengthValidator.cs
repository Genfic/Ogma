using System.Linq;
using FluentValidation;
using FluentValidation.Validators;

namespace Ogma3.Infrastructure.CustomValidators;

public class HashtagLengthValidator<T> : IPropertyValidator<T, string>
{
	private readonly uint _max;

	public HashtagLengthValidator(uint max)
	{
		_max = max;
	}

	public bool IsValid(ValidationContext<T> context, string value)
	{
		if (!value.Split(',').Any(t => t.Length > _max)) return true;

		context.MessageFormatter.AppendArgument("MaxLength", _max);
		return false;
	}

	public string Name => "HashtagLengthValidator";

	public string GetDefaultMessageTemplate(string errorCode)
		=> "No tag can be longer than {MaxLength} characters";
}

public static class HashtagLengthValidator
{
	public static IRuleBuilderOptions<T, string> HashtagsShorterThan<T>(this IRuleBuilder<T, string> ruleBuilder, uint max)
		=> ruleBuilder.SetValidator(new HashtagLengthValidator<T>(max));
}