using System;
using FluentValidation;
using FluentValidation.Validators;

namespace Ogma3.Infrastructure.CustomValidators;

public class HashtagCountValidator<T> : IPropertyValidator<T, string>
{
	private readonly uint _max;

	public HashtagCountValidator(uint max)
	{
		_max = max;
	}

	public bool IsValid(ValidationContext<T> context, string value)
	{
		if (Validate(value)) return true;

		context.MessageFormatter.AppendArgument("MaxElements", _max);
		return false;
	}

	public bool IsValid(string value) => Validate(value);

	private bool Validate(string value)
	{
		var span = value.Trim(',').AsSpan();

		if (span.Length == 0) return true;
		
		var count = 0;
		var wasComma = true;
		foreach (var ch in span)
		{
			if (ch == ',')
			{
				wasComma = true;
				continue;
			}

			if (wasComma && !char.IsWhiteSpace(ch))
			{
				count++;
				wasComma = false;
			}
			

			if (count > _max) return false;
		}

		return true;
	}

	public string Name => "HashtagCountValidator";

	public string GetDefaultMessageTemplate(string errorCode)
		=> "You can't use more than {MaxElements} tags.";
}

public static class HashtagCountValidatorExtension
{
	public static IRuleBuilderOptions<T, string> HashtagsFewerThan<T>(this IRuleBuilder<T, string> ruleBuilder, uint max)
		=> ruleBuilder.SetValidator(new HashtagCountValidator<T>(max));
}