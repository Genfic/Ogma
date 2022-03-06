using System.IO;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ogma3.Data;
using Ogma3.Data.Ratings;
using Ogma3.Infrastructure.CustomValidators;
using Ogma3.Infrastructure.CustomValidators.FileSizeValidator;
using Ogma3.Infrastructure.MediatR.Bases;
using Ogma3.Services.FileUploader;
using Utils.Extensions;

namespace Ogma3.Api.V1.Ratings.Commands;

public static class CreateRating
{
    public sealed record Command (string Name, string Description, bool BlacklistedByDefault, byte Order, IFormFile Icon) 
        : IRequest<ActionResult<RatingApiDto>>;

    public class CommandValidator : AbstractValidator<Command>
    {
        public CommandValidator()
        {
            RuleFor(r => r.Name)
                .MinimumLength(CTConfig.CRating.MinNameLength)
                .MaximumLength(CTConfig.CRating.MaxNameLength);
            RuleFor(r => r.Description)
                .MinimumLength(CTConfig.CRating.MinDescriptionLength)
                .MaximumLength(CTConfig.CRating.MaxDescriptionLength);
            RuleFor(r => r.Icon)
                .FileHasExtension(".svg")
                .FileSmallerThan(100 * 1024)
                .When(r => r.Icon is not null);
        }
    }
        
    public class Handler : BaseHandler, IRequestHandler<Command, ActionResult<RatingApiDto>>
    {
        private readonly ApplicationDbContext _context;
        private readonly ImageUploader _uploader;
        private readonly OgmaConfig _ogmaConfig;

        public Handler(ApplicationDbContext context, ImageUploader uploader, OgmaConfig ogmaConfig)
        {
            _context = context;
            _uploader = uploader;
            _ogmaConfig = ogmaConfig;
        }

        public async Task<ActionResult<RatingApiDto>> Handle(Command request, CancellationToken cancellationToken)
        {
            var (name, description, blacklistedByDefault, order, formFile) = request;
                
            var rating = new Rating
            {
                Name = name,
                Description = description,
                BlacklistedByDefault = blacklistedByDefault,
                Order = order
            };

            if (formFile is {Length: > 0})
            {
                var fileData = await _uploader.Upload(formFile, "ratings", name.Friendlify().ToUpper());
                rating.Icon = Path.Join(_ogmaConfig.Cdn, fileData.Path);
                rating.IconId = fileData.FileId;
            }
            
            _context.Ratings.Add(rating);

            await _context.SaveChangesAsync(cancellationToken);

            return CreatedAtAction(
                nameof(RatingsController.GetRating),
                nameof(RatingsController)[..^10],
                new { rating.Id },
                rating
            );
        }
    }
}