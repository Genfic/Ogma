using FluentValidation;
using FluentValidation.Validators;

namespace Ogma3.Infrastructure.CustomValidators;

public class HashtagLengthValidator<T>(uint max) : IPropertyValidator<T, string?>
{
	public bool IsValid(ValidationContext<T> context, string? value)
	{
		var valid = Validate(value);

		if (valid) return true;
			
		context.MessageFormatter.AppendArgument("MaxLength", max);
		return false;
	}

	public bool IsValid(string? value) => Validate(value);

	private bool Validate(string? value)
	{
		if (value is null) return true;
		
		var span = value.Trim(',').AsSpan();

		if (span.Length == 0) return true;
		
		var count = 0;
		var wasComma = false;
		foreach(var ch in span)
		{
			if (wasComma && char.IsWhiteSpace(ch)) continue;
			
			if (ch == ',')
			{
				wasComma = true;
				count = 0;
			}
			else
			{
				wasComma = false;
				count++;
			}

			if (count > max) return false;
		}
		
		return true;
	}

	public string Name => "HashtagLengthValidator";

	public string GetDefaultMessageTemplate(string errorCode)
		=> "No tag can be longer than {MaxLength} characters";
}

public static class HashtagLengthValidator
{
	public static IRuleBuilderOptions<T, string?> HashtagsShorterThan<T>(this IRuleBuilder<T, string?> ruleBuilder, uint max)
		=> ruleBuilder.SetValidator(new HashtagLengthValidator<T>(max));
}