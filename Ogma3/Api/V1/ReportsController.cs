using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ogma3.Data;
using Ogma3.Data.Reports;
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
                ContentType = itemType.ToString()
            };

            switch (itemType)
            {
                case EReportableContentTypes.Comment:
                    report.CommentId = itemId;
                    break;
                case EReportableContentTypes.User:
                    report.UserId = itemId;
                    break;
                case EReportableContentTypes.Story:
                    report.StoryId = itemId;
                    break;
                case EReportableContentTypes.Chapter:
                    report.ChapterId = itemId;
                    break;
                case EReportableContentTypes.Blogpost:
                    report.BlogpostId = itemId;
                    break;
                case EReportableContentTypes.Club:
                    report.ClubId = itemId;
                    break;
                default:
                    return BadRequest();
            }

            await _context.Reports.AddAsync(report);
            await _context.SaveChangesAsync();

            return new OkObjectResult(report);
        }

        public sealed record PostData(long ItemId, string Reason, EReportableContentTypes ItemType);
    }
}