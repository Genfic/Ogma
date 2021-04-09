using System.Linq;
using FluentValidation;

namespace Ogma3.Data.Blogposts
{
    public class BlogpostEditDtoValidation : AbstractValidator<BlogpostEditDto>
    {
        public BlogpostEditDtoValidation()
        {
            RuleFor(b => b.Title)
                .NotEmpty()
                .WithMessage("{PropertyName} must not be empty.")
                .Length(CTConfig.CBlogpost.MinTitleLength, CTConfig.CBlogpost.MaxTitleLength)
                .WithMessage("{PropertyName} must be between {MinLength} and {MaxLength} characters.");
            
            RuleFor(b => b.Body)
                .NotEmpty()
                .WithMessage("{PropertyName} must not be empty.")
                .Length(CTConfig.CBlogpost.MinBodyLength, CTConfig.CBlogpost.MaxBodyLength)
                .WithMessage("{PropertyName} must be between {MinLength} and {MaxLength} characters.");
            
            RuleFor(b => b.Tags)
                .Must(t => (t?.Split(',').Length ?? 0) < CTConfig.CBlogpost.MaxTagsAmount)
                .WithMessage($"Post can have no more than {CTConfig.CBlogpost.MaxTagsAmount} tags.")
                .Must(t => t?.Split(',')?.All(x => x.Length < CTConfig.CBlogpost.MaxTagLength) ?? true)
                .WithMessage($"No tag can be longer than {CTConfig.CBlogpost.MaxTagLength} characters.");
        }
    }
}