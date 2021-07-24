using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Infrastructure.ActionResults;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Services.UserService;
using Serilog;

namespace Ogma3.Api.V1.ChaptersReads.Commands
{
    public static class MarkChapterAsUnread
    {
        public sealed record Query(long Chapter, long Story) : IRequest<IActionResult>;

        public class MarkChapterAsReadHandler : IRequestHandler<Query, IActionResult>
        {
            private readonly ApplicationDbContext _context;
            private readonly long? _uid;

            public MarkChapterAsReadHandler(ApplicationDbContext context, IUserService userService)
            {
                _context = context;
                _uid = userService?.User?.GetNumericId();
            }

            public async Task<IActionResult> Handle(Query request, CancellationToken cancellationToken)
            {
                if (_uid is null) return new UnauthorizedResult();

                var (chapter, story) = request;

                var chaptersReadObj = await _context.ChaptersRead
                    .Where(cr => cr.StoryId == story)
                    .Where(cr => cr.UserId == _uid)
                    .FirstOrDefaultAsync(cancellationToken);

                if (chaptersReadObj is null) return new OkResult();

                chaptersReadObj.Chapters.Remove(chapter);

                if (chaptersReadObj.Chapters.Count < 1)
                {
                    _context.ChaptersRead.Remove(chaptersReadObj);
                }

                // Save
                try
                {
                    // BUG: Changes to the `chaptersReadObj.Chapters` HashSet do not persist https://github.com/npgsql/efcore.pg/issues/1927
                    await _context.SaveChangesAsync(cancellationToken);
                    return new OkObjectResult(new { Read = chaptersReadObj.Chapters });
                }
                catch (Exception e)
                {
                    Log.Error(e, "Exception occurred when marking chapter {Chapter} as unread by {User}", chapter,
                        _uid);
                    return new ServerErrorResult("Database remove error");
                }
            }
        }
    }
}