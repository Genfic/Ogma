using FluentValidation;
using FluentValidation.AspNetCore;
using FluentValidation.Internal;
using Humanizer;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Ogma3.Infrastructure.CustomValidators.FileSizeValidator;

public sealed class FileSizeClientValidator(IValidationRule rule, IRuleComponent component) : ClientValidatorBase(rule, component)
{
	public override void AddValidation(ClientModelValidationContext context)
	{
		var validator = (IFileSizeValidator)Validator;
		MergeAttribute(context.Attributes, "data-val", "true");
		MergeAttribute(context.Attributes, "data-val-filesize", GetErrorMessage(validator, context));
		MergeAttribute(context.Attributes, "data-val-filesize-max", validator.Max.ToString());
	}

	private string GetErrorMessage(IFileSizeValidator lengthVal, ModelValidationContextBase context)
	{
		var cfg = context.ActionContext.HttpContext.RequestServices.GetRequiredService<ValidatorConfiguration>();

		var formatter = cfg.MessageFormatterFactory()
			.AppendPropertyName(Rule.GetDisplayName(null))
			.AppendArgument("MaxFilesize", lengthVal.Max.Bytes());

		string message;
		try
		{
			message = Component.GetUnformattedErrorMessage();
		}
		catch (NullReferenceException)
		{
			message = "Maximum file size is {MaxFilesize}.";
		}

		message = formatter.BuildMessage(message);
		return message;
	}
}