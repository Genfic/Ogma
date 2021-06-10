using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using B2Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Ratings;
using Ogma3.Services.FileUploader;

namespace Ogma3.Api.V1
{
    [Route("api/[controller]", Name = nameof(RatingsController))]
    [ApiController]
    public class RatingsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ImageUploader _uploader;
        private readonly IB2Client _b2Client;
        private readonly IMapper _mapper;

        public RatingsController(ApplicationDbContext context, ImageUploader uploader, IB2Client b2Client, IMapper mapper)
        {
            _context = context;
            _uploader = uploader;
            _b2Client = b2Client;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IEnumerable<RatingApiDto>> GetRatings()
        {
            return await _context.Ratings
                .OrderBy(r => r.Order)
                .ProjectTo<RatingApiDto>(_mapper.ConfigurationProvider)
                .AsNoTracking()
                .ToListAsync();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PostRating([FromForm] PostData rating)
        {
            var r = new Rating
            {
                Name = rating.Name,
                Description = rating.Description,
                BlacklistedByDefault = rating.BlacklistedByDefault,
                Order = rating.Order
            };

            if (rating.Icon is {Length: > 0})
            {
                var fileData = await _uploader.Upload(rating.Icon, "ratings", rating.Name+"_rating");
                r.Icon = fileData.Path;
                r.IconId = fileData.FileId;
            }
            
            _context.Ratings.Add(r);

            try
            {
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
        
        [HttpPut("{id:long}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutRating([FromForm] PostData rating, long id)
        {
            var r = await _context.Ratings.FindAsync(id);

            r.Name = rating.Name;
            r.Description = rating.Description;
            r.BlacklistedByDefault = rating.BlacklistedByDefault;
            r.Order = rating.Order;
            
            if (rating.Icon is {Length: > 0})
            {
                if (r.IconId is not null && r.Icon is not null)
                {
                    await _b2Client.Files.Delete(r.IconId, r.Icon);
                }

                var fileData = await _uploader.Upload(rating.Icon, "ratings", rating.Name+"_rating");
                r.Icon = fileData.Path;
                r.IconId = fileData.FileId;
            }

            try
            {
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpDelete("{id:long}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteRating(long id)
        {
            var r = await _context.Ratings.FindAsync(id);
            _context.Ratings.Remove(r);            
            
            if (r.Icon is not null && r.IconId is not null)
            {
                await _b2Client.Files.Delete(r.IconId, r.Icon);
            }
            
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
            
            return Ok();
        }
        
        public record PostData
        {
            public string Name { get; init; }
            public string Description { get; init; }
            public bool BlacklistedByDefault { get; init; }
            public byte Order { get; init; }
            public IFormFile Icon { get; init; }
        }
    }
}
