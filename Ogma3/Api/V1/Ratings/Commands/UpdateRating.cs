using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Ratings;
using Ogma3.Infrastructure.CustomValidators;
using Ogma3.Infrastructure.CustomValidators.FileSizeValidator;
using Ogma3.Services.FileUploader;
using Utils.Extensions;

namespace Ogma3.Api.V1.Ratings.Commands;

public static class UpdateRating
{
    public sealed record Command (long Id, string Name, string Description, bool BlacklistedByDefault, byte Order, IFormFile Icon) 
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

    public class Handler : IRequestHandler<Command, ActionResult<RatingApiDto>>
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
            var (id, name, description, blacklistedByDefault, order, formFile) = request;

            var rating = await _context.Ratings
                .Where(r => r.Id == id)
                .FirstOrDefaultAsync(cancellationToken);

            if (rating is null) return new NotFoundResult();

            rating.Name = name;
            rating.Description = description;
            rating.BlacklistedByDefault = blacklistedByDefault;
            rating.Order = order;

            if (formFile is {Length: > 0})
            {
                if (!string.Equals(name, rating.Name, StringComparison.OrdinalIgnoreCase))
                {
                    await _uploader.Delete( rating.Icon, rating.IconId, cancellationToken);
                }
                var fileData = await _uploader.Upload(formFile, "ratings", name.Friendlify().ToUpper());
                rating.Icon = Path.Join(_ogmaConfig.Cdn, fileData.Path);
                rating.IconId = fileData.FileId;
            }

            await _context.SaveChangesAsync(cancellationToken);

            return new CreatedAtActionResult(
                nameof(RatingsController.GetRating),
                nameof(RatingsController)[..^10],
                new { rating.Id },
                rating
            );
        }
    }
}