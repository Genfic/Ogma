using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Votes;

namespace Ogma3.Api.V1
{
    [Route("api/[controller]", Name = nameof(VotesController))]
    [ApiController]
    public class VotesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly OgmaUserManager _userManager;


        public VotesController(ApplicationDbContext context, OgmaUserManager userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET api/votes/5
        [HttpGet("{storyId}")]
        public async Task<CountReturn> GetVotes(long storyId)
        {
            var user = await _userManager.GetUserAsync(User);
            
            var count = await _context.Votes
                .AsNoTracking()
                .CountAsync(v => v.StoryId == storyId);
            var didUserVote = await _context.Votes
                .AsNoTracking()
                .AnyAsync(v => v.User == user && v.StoryId == storyId);

            return new CountReturn
            {
                Count = count,
                DidVote = didUserVote
            };
        }
        
        // POST api/votes
        [HttpPost]
        [Authorize]
        public async Task<ActionResult> PostVote([FromBody] VoteData data)
        {
            var user = await _userManager.GetUserAsync(User);
            var story = await _context.Stories
                .Where(s => s.Id == data.StoryId)
                .Include(s => s.Votes)
                .FirstOrDefaultAsync();
            var vote = await _context.Votes
                .FirstOrDefaultAsync(v => v.User == user && v.StoryId == data.StoryId);
            var didVote = false;

            if (story == null) return NotFound();

            // Check if the vote already exists
            if (vote == null)
            {
                story.Votes.Add(new Vote
                {
                    User = user
                });
                didVote = true;
            }
            else
            {
                story.Votes.Remove(vote);
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

            var res = new CountReturn
            {
                Count = story.Votes.Count,
                DidVote = didVote
            };
            return new OkObjectResult(res);
        }

        public sealed record CountReturn
        {
            public int Count { get; init; }
            public bool DidVote { get; init; }
        }

        public sealed record VoteData
        {
            public long StoryId { get; init; }
        }
    }
}