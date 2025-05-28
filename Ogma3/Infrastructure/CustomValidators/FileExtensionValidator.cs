using FluentValidation;
using FluentValidation.Validators;
using Immediate.Validations.Shared;

namespace Ogma3.Infrastructure.CustomValidators;

public sealed class FileExtensionAttribute(params string[] allowedExtensions): ValidatorAttribute
{
	public static bool ValidateProperty(IFormFile? value, string[] allowedExtensions)
	{
		if (value is null)
		{
			return true;
		}

		var extension = Path.GetExtension(value.FileName);
		return allowedExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase);
	}

	public static string DefaultMessage => "{PropertyName} must be a file with one of the following extensions: {AllowedExtensionsValue}";
}

public sealed class FileExtensionValidator<T>(string[] allowedExtensions) : PropertyValidator<T, IFormFile?>
{
	public override bool IsValid(ValidationContext<T> context, IFormFile? value)
	{
		if (value is null) return true;

		var extension = Path.GetExtension(value.FileName);
		if (allowedExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase)) return true;

		context.MessageFormatter.AppendArgument("AllowedExtensions", string.Join(", ", allowedExtensions));
		context.MessageFormatter.AppendArgument("ActualExtension", extension);
		return false;
	}

	public override string Name => "FileSizeValidator";

	protected override string GetDefaultMessageTemplate(string errorCode)
		=> "Only allowed extensions are {AllowedExtensions}. Your file is {ActualExtension}.";
}

public static class FileExtensionValidatorExtension
{
	public static IRuleBuilderOptions<T, IFormFile?> FileHasExtension<T>(this IRuleBuilder<T, IFormFile?> ruleBuilder,
		params string[] allowedExtensions)
		=> ruleBuilder.SetValidator(new FileExtensionValidator<T>(allowedExtensions));
}