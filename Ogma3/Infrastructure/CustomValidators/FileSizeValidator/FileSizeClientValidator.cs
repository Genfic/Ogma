using FluentValidation;
using FluentValidation.AspNetCore;
using FluentValidation.Internal;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Ogma3.Infrastructure.CustomValidators.FileSizeValidator
{
    public class FileSizeClientValidator : ClientValidatorBase
    {
        // TODO: Make this shit work somehow
        public FileSizeClientValidator(IValidationRule rule, IRuleComponent component) : base(rule, component)
        { }

        public override void AddValidation(ClientModelValidationContext context)
        {
            MergeAttribute(context.Attributes, "fuck-you-fluent-validation", "fuuuuuuuuuck yooooooou");
        }
    }
}