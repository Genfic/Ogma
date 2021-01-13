using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Ogma3.Infrastructure.Attributes
{
    public class AllowedExtensionsAttribute : ValidationAttribute, IClientModelValidator
    {
        private readonly string[] _extensions;
        private readonly string _message;

        public AllowedExtensionsAttribute(string[] extensions)
        {
            _extensions = extensions;
            _message = $"The only allowed extensions are: {string.Join(", ", _extensions)}";
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            switch (value)
            {
                case null:
                    return ValidationResult.Success;
                
                case IFormFile file:
                {
                    var extension = Path.GetExtension(file.FileName).ToLower();
                    var fileType = file.ContentType.Split('/')[0].ToLower();
                
                    if (!Array.Exists(_extensions, e => e == extension) || fileType != "image")
                        return new ValidationResult(_message);
                    
                    return ValidationResult.Success;
                }
                default:
                    return new ValidationResult("Object is not a valid file.");
            }

        }

        public void AddValidation(ClientModelValidationContext context)
        {
            context.Attributes.Add("data-val-fileextensions", _message);
            context.Attributes.Add("data-val-fileextensions-extensions", string.Join(',', _extensions));
            context.Attributes.Add("accept", string.Join(',', _extensions));
        }
    }
}