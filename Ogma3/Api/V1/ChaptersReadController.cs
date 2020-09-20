using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Models;

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
            var user = await _userManager.GetUserAsync(User);
            var chaptersRead = await _context.ChaptersRead
                .Where(cr => cr.StoryId == story && cr.User == user)
                .Select(cr => cr.Chapters)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (chaptersRead == null) return NotFound();
            
            return new OkObjectResult(new { Read = chaptersRead });
        }
        
        // POST api/chaptersread
        [HttpPost]
        [Authorize]
        public async Task<ActionResult> PostChaptersRead(ChaptersReadPost post)
        {
            var user = await _userManager.GetUserAsync(User);
            var chaptersReadObj = await _context.ChaptersRead
                .FirstOrDefaultAsync(cr => cr.StoryId == post.Story && cr.UserId == user.Id);
            
            // If no read list exists yet, create one with the chapter read
            if (chaptersReadObj == null)
            {
                var newCr = new ChaptersRead
                {
                    StoryId = post.Story,
                    User = user,
                    Chapters = new List<long>{ post.Chapter }
                };
                await _context.ChaptersRead.AddAsync(newCr);
            }
            else // just update the existing one
            {
                if (chaptersReadObj.Chapters.Contains(post.Chapter))
                {
                    chaptersReadObj.Chapters.Remove(post.Chapter);
                }
                else
                {
                    chaptersReadObj.Chapters.Add(post.Chapter);
                }
            }

            // Save
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return StatusCode(500);
            }

            return new OkObjectResult(new { Read = chaptersReadObj?.Chapters });
        }

        public class ChaptersReadPost
        {
            public long Chapter { get; set; }
            public long Story { get; set; }
        }
    }
}