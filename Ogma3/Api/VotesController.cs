using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Ogma3.Data;
using Ogma3.Data.DTOs;
using Ogma3.Data.Models;

namespace Ogma3.Api
{
    [Route("api/[controller]", Name = nameof(VotesController))]
    [ApiController]
    public class VotesController : Controller
    {
        private ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        

        public VotesController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET api/votes/5
        [HttpGet("{id}")]
        public async Task<int> GetVotes(int poolId)
        {
            var pool = await _context.VotePools.FindAsync(poolId);
            return pool.Votes.Count;
        }
        
        // POST api/votes
        [HttpPost]
        [Authorize]
        public async Task<ActionResult> PostVote(VoteFromApiDTO data)
        {
            var vote = new Vote
            {
                User = await _userManager.GetUserAsync(User)
            };
            var pool = await _context.VotePools.FindAsync(data.VotePool);
            pool.Votes.Add(vote);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return StatusCode(500);
            }

            return Ok();
        }
        
        // DELETE api/votes/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult> DeleteVote(int id)
        {
            var vote = await _context.Votes.FindAsync(id);
            if (vote == null)
            {
                return NotFound();
            }

            _context.Votes.Remove(vote);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}