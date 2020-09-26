using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Humanizer;

namespace Ogma3.Infrastructure.Attributes
{
    public class MaxFileSizeAttribute : ValidationAttribute
    {
        private readonly int _maxFileSize;
        public MaxFileSizeAttribute(int maxFileSize)
        {
            _maxFileSize = maxFileSize;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;
            
            if (!(value is IFormFile file)) 
                return new ValidationResult("Object is not a valid file.");
            
            return file.Length > _maxFileSize 
                ? new ValidationResult($"Maximum allowed file size is {_maxFileSize.Bytes().Humanize()}") 
                : ValidationResult.Success;
        }
    }
}