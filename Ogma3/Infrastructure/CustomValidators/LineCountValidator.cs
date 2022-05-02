using System;
using FluentValidation;
using FluentValidation.Validators;

namespace Ogma3.Infrastructure.CustomValidators;

public class LineCountValidator<T> : PropertyValidator<T, string>
{
	private readonly uint _max;

	public LineCountValidator(uint max) => _max = max;

	public override bool IsValid(ValidationContext<T> context, string value)
	{
		if (value.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None).Length <= _max)
		{
			return true;
		}

		context.MessageFormatter.AppendArgument("MaxLines", _max);
		return false;
	}

	public override string Name => "LineCountValidator";

	protected override string GetDefaultMessageTemplate(string errorCode)
		=> "You can't have more than {MaxElements} lines.";
}

public static class LineCountValidatorExtension
{
	public static IRuleBuilderOptions<T, string> MaximumLines<T>(this IRuleBuilder<T, string> ruleBuilder, uint max)
		=> ruleBuilder.SetValidator(new LineCountValidator<T>(max));
}