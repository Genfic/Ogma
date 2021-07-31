using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using B2Net;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Ratings;
using Ogma3.Services.FileUploader;
using Utils.Extensions;

namespace Ogma3.Api.V1.Ratings.Commands
{
    public static class UpdateRating
    {
        public sealed record Query (long Id, string Name, string Description, bool BlacklistedByDefault, byte Order, IFormFile Icon) 
            : IRequest<ActionResult<RatingApiDto>>;

        public class Handler : IRequestHandler<Query, ActionResult<RatingApiDto>>
        {
            private readonly ApplicationDbContext _context;
            private readonly ImageUploader _uploader;
            private readonly IB2Client _b2Client;

            public Handler(ApplicationDbContext context, ImageUploader uploader, IB2Client b2Client)
            {
                _context = context;
                _uploader = uploader;
                _b2Client = b2Client;
            }

            public async Task<ActionResult<RatingApiDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                var (id, name, description, blacklistedByDefault, order, formFile) = request;

                var rating = await _context.Ratings
                    .Where(r => r.Id == id)
                    .FirstOrDefaultAsync(cancellationToken);

                rating.Name = name;
                rating.Description = description;
                rating.BlacklistedByDefault = blacklistedByDefault;
                rating.Order = order;

                if (formFile is {Length: > 0})
                {
                    if (!string.Equals(name, rating.Name, StringComparison.OrdinalIgnoreCase))
                    {
                        await _b2Client.Files.Delete(rating.IconId, rating.Icon, cancellationToken);
                    }
                    var fileData = await _uploader.Upload(formFile, "ratings", $"{name.Friendlify().ToUpper()}_rating");
                    rating.Icon = fileData.Path;
                    rating.IconId = fileData.FileId;
                }
            
                _context.Ratings.Update(rating);

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