using FluentValidation;
using FluentValidation.Validators;
using Humanizer;
using Microsoft.AspNetCore.Http;

namespace Ogma3.Infrastructure.CustomValidators
{
    public class FileSizeValidator<T> : PropertyValidator<T, IFormFile>
    {
        private readonly uint _max;

        public FileSizeValidator(uint max)
        {
            _max = max;
        }

        public override bool IsValid(ValidationContext<T> context, IFormFile value)
        {
            if (value is null) return true;
            if (value.Length <= _max) return true;

            context.MessageFormatter.AppendArgument("MaxFilesize", _max.Bytes());
            context.MessageFormatter.AppendArgument("ActualFilesize", value.Length.Bytes());
            return false;
        }

        public override string Name => "FileSizeValidator";
        
        protected override string GetDefaultMessageTemplate(string errorCode)
            => "Maximum file size is {MaxFilesize}. Yours is {ActualFilesize}";
    }

    public static class FileSizeValidatorExtension
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ruleBuilder"></param>
        /// <param name="max">Maximum file size in bytes</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IRuleBuilderOptions<T, IFormFile> FileSmallerThan<T>(this IRuleBuilder<T, IFormFile> ruleBuilder, uint max)
            => ruleBuilder.SetValidator(new FileSizeValidator<T>(max));
    }
}