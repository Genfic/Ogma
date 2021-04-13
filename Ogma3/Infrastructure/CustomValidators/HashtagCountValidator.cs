using FluentValidation;
using FluentValidation.Validators;

namespace Ogma3.Infrastructure.CustomValidators
{
    public class HashtagCountValidator<T> : PropertyValidator<T, string>
    {
        private readonly uint _max;

        public HashtagCountValidator(uint max)
        {
            _max = max;
        }

        public override bool IsValid(ValidationContext<T> context, string value)
        {
            if (value.Split(',').Length <= _max) return true;
            
            context.MessageFormatter.AppendArgument("MaxElements", _max);
            return false;
        }

        public override string Name => "HashtagCountValidator";

        protected override string GetDefaultMessageTemplate(string errorCode)
            => "You can't use more than {MaxElements} tags.";
    }

    public static class HashtagCountValidatorExtension
    {
        public static IRuleBuilderOptions<T, string> HashtagsFewerThan<T>(this IRuleBuilder<T, string> ruleBuilder, uint max)
            => ruleBuilder.SetValidator(new HashtagCountValidator<T>(max));
    }
}