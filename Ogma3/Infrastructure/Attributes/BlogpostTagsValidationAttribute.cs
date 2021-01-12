using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Ogma3.Infrastructure.Attributes
{
    public class BlogpostTagsValidationAttribute : ValidationAttribute
    {
        private readonly int _count;
        private readonly int _maxLength;
        
        public BlogpostTagsValidationAttribute(int count, int maxLength = 50)
        {
            _count = count;
            _maxLength = maxLength;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is null) 
                return ValidationResult.Success;

            var tags = ((string) value).Split(',');

            if (tags.Length > _count)
                return new ValidationResult($"The amount of tags needs to be less than {_count}");

            if (tags.Any(t => t.Length > _maxLength))
                return new ValidationResult($"No tag can be longer than {_maxLength} characters");
            
            return ValidationResult.Success;
        }
    }
}