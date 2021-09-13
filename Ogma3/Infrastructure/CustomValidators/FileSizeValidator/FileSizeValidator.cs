using FluentValidation;
using FluentValidation.Validators;
using Humanizer;
using Microsoft.AspNetCore.Http;

namespace Ogma3.Infrastructure.CustomValidators.FileSizeValidator
{
    public class FileSizeValidator<T> : PropertyValidator<T, IFormFile>, IFileSizeValidator
    {
        public uint Max { get; }
        public FileSizeValidator(uint max) => Max = max;

        public override bool IsValid(ValidationContext<T> context, IFormFile value)
        {
            if (value is null) return true;
            if (value.Length <= Max) return true;

            context.MessageFormatter
                .AppendArgument("MaxFilesize", Max.Bytes());
            
            return false;
        }

        public override string Name => "FileSizeValidator";
        
        protected override string GetDefaultMessageTemplate(string errorCode)
            => "Maximum file size is {MaxFilesize}.";
    }
    
    public static class FileSizeValidatorExtension
    {
        /// <param name="ruleBuilder"></param>
        /// <param name="max">Maximum file size in bytes</param>
        public static IRuleBuilderOptions<T, IFormFile> FileSmallerThan<T>(this IRuleBuilder<T, IFormFile> ruleBuilder, uint max)
            => ruleBuilder.SetValidator(new FileSizeValidator<T>(max));
    }

    public interface IFileSizeValidator : IPropertyValidator
    {
        public uint Max { get; }
    }
}