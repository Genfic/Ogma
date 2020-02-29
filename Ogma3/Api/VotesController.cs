using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.DTOs;
using Ogma3.Data.Models;

namespace Ogma3.Api
{
    [Route("api/[controller]", Name = nameof(VotesController))]
    [ApiController]
    public class VotesController : Controller
    {
        private readonly ApplicationDbContext _context;
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
            return await _context.VotePools.CountAsync(vp => vp.Id == poolId);
        }

        // GET api/votes/uservote/5
        [HttpGet("uservote/{id}")]
        public async Task<bool> GetUserVote(int poolId)
        {
            var user = await _userManager.GetUserAsync(User);
            return await _context.Votes.AnyAsync(v => v.User == user && v.VotePoolId == poolId);
        }
        
        // POST api/votes
        [HttpPost]
        [Authorize]
        public async Task<ActionResult> PostVote(VoteFromApiDTO data)
        {
            var user = await _userManager.GetUserAsync(User);
            var pool = await _context.VotePools.FindAsync(data.VotePool);
            var vote = await _context.Votes
                .Where(v => v.User == user && v.VotePoolId == data.VotePool)
                .FirstOrDefaultAsync();

            Console.WriteLine(vote == null);
            
            if (vote == null)
            {
                pool.Votes.Add(new Vote {
                    User = await _userManager.GetUserAsync(User)
                });
            }
            else
            {
                pool.Votes.Remove(vote);
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return StatusCode(500);
            }
            
            return new OkObjectResult(await _context.Votes.CountAsync(v => v.VotePoolId == data.VotePool));
        }
    }
}