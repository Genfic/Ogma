using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Models;
using Ogma3.Infrastructure.Extensions;

namespace Ogma3.Api.V1
{
    [Route("api/[controller]", Name = nameof(ChaptersReadController))]
    [ApiController]
    public class ChaptersReadController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly OgmaUserManager _userManager;


        public ChaptersReadController(ApplicationDbContext context, OgmaUserManager userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET api/chaptersread/5
        [HttpGet("{story}")]
        [Authorize]
        public async Task<ActionResult<List<long>>> GetChaptersRead(long story)
        {
            var uid = User.GetNumericId();
            var chaptersRead = await _context.ChaptersRead
                .Where(cr => cr.StoryId == story)
                .Where(cr => cr.UserId == uid)
                .Select(cr => cr.Chapters)
                .FirstOrDefaultAsync();

            if (chaptersRead == null) return NoContent();
            
            return new OkObjectResult(new { Read = chaptersRead });
        }
        
        // POST api/chaptersread
        [HttpPost]
        [Authorize]
        public async Task<ActionResult> PostChaptersRead(ChaptersReadPost post)
        {
            var uid = User.GetNumericId();
            if (uid is null) return Unauthorized();
            
            var (chapter, story) = post;
            
            var chaptersReadObj = await _context.ChaptersRead
                .FirstOrDefaultAsync(cr => cr.StoryId == story && cr.UserId == uid);
            
            // If no read list exists yet, create one with the chapter read
            List<long> res;
            if (chaptersReadObj is null)
            {
                var newCr = new ChaptersRead
                {
                    StoryId = story,
                    UserId = (long) uid,
                    Chapters = new List<long> { chapter }
                };
                await _context.ChaptersRead.AddAsync(newCr);
                res = newCr.Chapters;
            }
            else // just update the existing one
            {
                if (chaptersReadObj.Chapters.Contains(chapter))
                {
                    if (chaptersReadObj.Chapters.Count <= 1)
                    {
                        _context.ChaptersRead.Remove(chaptersReadObj);
                        res = new List<long>();
                    }
                    else
                    {
                        chaptersReadObj.Chapters.Remove(chapter);
                        res = chaptersReadObj.Chapters;
                    }
                }
                else
                {
                    chaptersReadObj.Chapters.Add(chapter);
                    res = chaptersReadObj.Chapters;
                }
            }

            // Save
            try
            {
                await _context.SaveChangesAsync();
                return new OkObjectResult(new { Read = res });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return StatusCode(500);
            }
        }

        public sealed record ChaptersReadPost(long Chapter, long Story);
    }
}