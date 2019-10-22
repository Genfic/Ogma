using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace Ogma3.Services.Attributes
{
    public class AllowedExtensionsAttribute : ValidationAttribute
    {
        private readonly string[] _extensions;

        public AllowedExtensionsAttribute(string[] extensions)
        {
            _extensions = extensions;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is IFormFile file)
            {
                var extension = Path.GetExtension(file.FileName);
                
                if (
                    !Array.Exists(_extensions, e => e == extension.ToLower())
                    || file.ContentType.Split('/')[0].ToLower() != "image"
                )
                {
                    return new ValidationResult("This file extension is not allowed.");
                }
            }
            else
            {
                return new ValidationResult("Object does not implement IFormFile interface.");
            }

            return ValidationResult.Success;
        }
    }
}