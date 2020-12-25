using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ogma3.Data;
using Ogma3.Data.Models;
using Ogma3.Infrastructure.Attributes;
using Ogma3.Infrastructure.Extensions;

namespace Ogma3.Api.V1
{
    [Route("api/[controller]", Name = nameof(ReportsController))]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        
        public ReportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET
        [HttpGet]
        public async Task GetReportsAsync()
        {

        }

        // POST
        [HttpPost]
        [Authorize]
        [Throttle(Count = 3, TimeUnit = TimeUnit.Hour)]
        public async Task<ActionResult> PostReportsAsync([FromBody] PostData data)
        {
            var uid = User.GetNumericId();
            if (uid is null) return Unauthorized();

            var (itemId, reason, itemType) = data;
            var report = new Report
            {
                Reason = reason,
                ReporterId = (long)uid,
                ContentType = itemType
            };

            switch (itemType)
            {
                case nameof(Comment):
                    report.CommentId = itemId;
                    break;
                case nameof(OgmaUser):
                    report.UserId = itemId;
                    break;
                case nameof(Story):
                    report.StoryId = itemId;
                    break;
                case nameof(Chapter):
                    report.ChapterId = itemId;
                    break;
                case nameof(Blogpost):
                    report.BlogpostId = itemId;
                    break;
                case nameof(Club):
                    report.ClubId = itemId;
                    break;
                default:
                    return BadRequest();
            }

            await _context.Reports.AddAsync(report);
            await _context.SaveChangesAsync();

            return new OkObjectResult(report);
        }

        public sealed record PostData(long ItemId, string Reason, string ItemType);
    }
}