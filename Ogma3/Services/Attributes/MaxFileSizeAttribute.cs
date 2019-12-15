using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Utils;

namespace Ogma3.Services.Attributes
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
                return new ValidationResult("Object does not implement IFormFile interface.");
            
            return file.Length > _maxFileSize ? new ValidationResult(GetErrorMessage()) : ValidationResult.Success;
        }

        private string GetErrorMessage()
        {
            var maxS = UnitConverters.FormatBytes(_maxFileSize);
            return $"Maximum allowed file size is {maxS}";
        }
    }
}