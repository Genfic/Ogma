using FluentValidation;
using FluentValidation.Validators;

namespace Ogma3.Infrastructure.CustomValidators;

public class LineCountValidator<T>(uint max) : PropertyValidator<T, string?>
{
	public override bool IsValid(ValidationContext<T> context, string? value)
	{
		if (value is null) return true;
		
		// TODO: Optimize this
		if (value.Split(["\r\n", "\r", "\n"], StringSplitOptions.None).Length <= max)
		{
			return true;
		}

		context.MessageFormatter.AppendArgument("MaxLines", max);
		return false;
	}

	public override string Name => "LineCountValidator";

	protected override string GetDefaultMessageTemplate(string errorCode)
		=> "You can't have more than {MaxElements} lines.";
}

public static class LineCountValidatorExtension
{
	public static IRuleBuilderOptions<T, string?> MaximumLines<T>(this IRuleBuilder<T, string?> ruleBuilder, uint max)
		=> ruleBuilder.SetValidator(new LineCountValidator<T>(max));
}