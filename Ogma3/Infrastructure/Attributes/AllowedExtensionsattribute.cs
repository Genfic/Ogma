using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace Ogma3.Infrastructure.Attributes
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
            switch (value)
            {
                case null:
                    return ValidationResult.Success;
                
                case IFormFile file:
                {
                    var extension = Path.GetExtension(file.FileName).ToLower();
                    var fileType = file.ContentType.Split('/')[0].ToLower();
                
                    if (!Array.Exists(_extensions, e => e == extension) || fileType != "image")
                        return new ValidationResult("This file extension is not allowed.");
                    
                    return ValidationResult.Success;
                }
                default:
                    return new ValidationResult("Object is not a valid file.");
            }

        }
    }
}