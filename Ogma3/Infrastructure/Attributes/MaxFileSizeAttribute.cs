using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Humanizer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Ogma3.Infrastructure.Attributes;

public class MaxFileSizeAttribute : ValidationAttribute, IClientModelValidator
{
	private readonly int _maxFileSize;
	private readonly string _message;

	public MaxFileSizeAttribute(int maxFileSize)
	{
		_maxFileSize = maxFileSize;
		_message = $"Maximum allowed file size is {_maxFileSize.Bytes().Humanize("##.##", CultureInfo.InvariantCulture)}";
	}

	protected override ValidationResult IsValid(object value, ValidationContext validationContext)
	{
		if (value == null)
			return ValidationResult.Success;

		if (value is not IFormFile file)
			return new ValidationResult("Object is not a valid file.");

		return file.Length > _maxFileSize
			? new ValidationResult(_message)
			: ValidationResult.Success;
	}

	public void AddValidation(ClientModelValidationContext context)
	{
		context.Attributes.Add("data-val", "true");
		context.Attributes.Add("data-val-filesize-max", _maxFileSize.ToString());
		context.Attributes.Add("data-val-filesize", _message);
	}
}