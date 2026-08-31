using FluentValidation;
using FluentValidation.Validators;

namespace Ogma3.Infrastructure.CustomValidators;

public sealed class HashtagCountValidator<T>(uint max) : IPropertyValidator<T, string?>
{
	public bool IsValid(ValidationContext<T> context, string? value)
	{
		if (Validate(value)) return true;

		context.MessageFormatter.AppendArgument("MaxElements", max);
		return false;
	}

	public bool IsValid(string? value) => Validate(value);

	private bool Validate(string? value)
	{
		if (value is null)
		{
			return true;
		}

		var span = value.Trim(',').AsSpan();
		if (span.IsEmpty)
		{
			return true;
		}

		var count = 0;
		foreach (var segment in span.Split(','))
		{
			var tag = span[segment].Trim();
			if (tag.IsEmpty)
			{
				continue;
			}

			count++;

			if (count > max)
			{
				return false;
			}
		}

		return true;
	}

	public string Name => "HashtagCountValidator";

	public string GetDefaultMessageTemplate(string errorCode)
		=> "You can't use more than {MaxElements} tags.";
}

public static class HashtagCountValidatorExtension
{
	public static IRuleBuilderOptions<T, string?> HashtagsFewerThan<T>(this IRuleBuilder<T, string?> ruleBuilder, uint max)
		=> ruleBuilder.SetValidator(new HashtagCountValidator<T>(max));
}