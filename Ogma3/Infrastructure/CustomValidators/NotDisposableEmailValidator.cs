using FluentValidation;
using FluentValidation.Validators;
using Ogma3.Services.EmailBlocklistProvider;

namespace Ogma3.Infrastructure.CustomValidators;

public sealed class NotDisposableEmailValidator<T>(IEmailBlocklistProvider provider) : PropertyValidator<T, string?>
{
	public override bool IsValid(ValidationContext<T> context, string? value)
	{
		if (string.IsNullOrWhiteSpace(value)) return true;

		var atIndex = value.LastIndexOf('@');

		if (atIndex == -1 || atIndex == value.Length - 1) return true;

		var domain = value.AsSpan(atIndex + 1);
		return !provider.IsDisposable(domain.ToString());
	}

	public override string Name => "NotDisposableEmailValidator";

	protected override string GetDefaultMessageTemplate(string errorCode)
		=> "Email not supported.";
}

public static class NotDisposableEmailValidatorExtension
{
	public static IRuleBuilderOptions<T, string?> NotDisposable<T>(this IRuleBuilder<T, string?> ruleBuilder, IEmailBlocklistProvider provider)
		=> ruleBuilder.SetValidator(new NotDisposableEmailValidator<T>(provider));
}