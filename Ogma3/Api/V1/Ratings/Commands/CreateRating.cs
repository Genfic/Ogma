using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ogma3.Data;
using Ogma3.Data.Ratings;
using Ogma3.Services.FileUploader;
using Utils.Extensions;

namespace Ogma3.Api.V1.Ratings.Commands
{
    public static class CreateRating
    {
        public sealed record Query (string Name, string Description, bool BlacklistedByDefault, byte Order, IFormFile Icon) 
            : IRequest<ActionResult<RatingApiDto>>;

        public class Handler : IRequestHandler<Query, ActionResult<RatingApiDto>>
        {
            private readonly ApplicationDbContext _context;
            private readonly ImageUploader _uploader;

            public Handler(ApplicationDbContext context, ImageUploader uploader)
            {
                _context = context;
                _uploader = uploader;
            }

            public async Task<ActionResult<RatingApiDto>> Handle(Query request, CancellationToken cancellationToken)
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
                    var fileData = await _uploader.Upload(formFile, "ratings", $"{name.Friendlify().ToUpper()}_rating");
                    rating.Icon = fileData.Path;
                    rating.IconId = fileData.FileId;
                }
            
                _context.Ratings.Add(rating);

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
}