using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.CommentsThreads;
using Ogma3.Infrastructure.Extensions;

namespace Ogma3.Api.V1
{
    [Route("api/[controller]", Name = nameof(SubscriptionController))]
    [ApiController]
    public class SubscriptionController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SubscriptionController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("thread")]
        public async Task<ActionResult<bool>> IsSubscribedToThreadAsync([FromQuery] long threadId)
        {
            var uid = User.GetNumericId();
            return await _context.CommentsThreadSubscribers
                .Where(cts => cts.OgmaUserId == uid)
                .Where(cts => cts.CommentsThreadId == threadId)
                .AnyAsync();
        }

        [HttpPost("thread")]
        [Authorize]
        public async Task<ActionResult<bool>> SubscribeThreadAsync([FromBody] ThreadData data)
        {
            var uid = User.GetNumericId();
            if (uid is null) return Unauthorized();

            await _context.CommentsThreadSubscribers
                .AddAsync(new CommentsThreadSubscriber
                {
                    OgmaUserId = (long) uid,
                    CommentsThreadId = data.ThreadId
                });
            await _context.SaveChangesAsync();
            
            return true;
        }
        
        [HttpDelete("thread")]
        [Authorize]
        public async Task<ActionResult<bool>> UnsubscribeThreadAsync([FromBody] ThreadData data)
        {
            var uid = User.GetNumericId();
            if (uid is null) return Unauthorized();

            var threadSubscriber = await _context.CommentsThreadSubscribers
                .Where(cts => cts.OgmaUserId == uid)
                .Where(cts => cts.CommentsThreadId == data.ThreadId)
                .FirstOrDefaultAsync();

            if (threadSubscriber is null) return true;
            
            _context.CommentsThreadSubscribers.Remove(threadSubscriber);
            await _context.SaveChangesAsync();

            return false;
        }

        public record ThreadData(long ThreadId);

        [HttpGet]
        public string Ping() => "pong!";
    }
}