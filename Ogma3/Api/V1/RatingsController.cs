using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using B2Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.DTOs;
using Ogma3.Data.Models;
using Ogma3.Services;
using Ogma3.Services.FileUploader;

namespace Ogma3.Api.V1
{
    [Route("api/[controller]", Name = nameof(RatingsController))]
    [ApiController]
    public class RatingsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly FileUploader _uploader;
        private readonly IB2Client _b2Client;

        public RatingsController(ApplicationDbContext context, FileUploader uploader, IB2Client b2Client)
        {
            _context = context;
            _uploader = uploader;
            _b2Client = b2Client;
        }

        [HttpGet]
        public async Task<IEnumerable<RatingApiDto>> GetRatings()
        {
            var list = await _context.Ratings
                .OrderBy(r => r.Id)
                .AsNoTracking()
                .ToListAsync();
            return list.Select(RatingApiDto.FromRating);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PostRating([FromForm] PostData rating)
        {
            var r = new Rating
            {
                Name = rating.Name,
                Description = rating.Description,
            };

            if (rating.Icon != null && rating.Icon.Length > 0)
            {
                var fileData = await _uploader.Upload(rating.Icon, "ratings", rating.Name+"_rating");
                r.Icon = fileData.Path;
                r.IconId = fileData.FileId;
            }
            
            await _context.Ratings.AddAsync(r);

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
        
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutRating([FromForm] PostData rating, long id)
        {
            var r = await _context.Ratings.FindAsync(id);

            r.Name = rating.Name;
            r.Description = rating.Description;
            
            if (rating.Icon != null && rating.Icon.Length > 0)
            {
                await _b2Client.Files.Delete(r.IconId, r.Icon);
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

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteRating(long id)
        {
            var r = await _context.Ratings.FindAsync(id);
            _context.Ratings.Remove(r);            
            
            if (r.Icon != null && r.IconId != null)
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
        
        public class PostData
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public IFormFile Icon { get; set; }
        }
    }
}
