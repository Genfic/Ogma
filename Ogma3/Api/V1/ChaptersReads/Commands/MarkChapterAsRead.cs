using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Chapters;
using Ogma3.Infrastructure.ActionResults;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Services.UserService;
using Serilog;

namespace Ogma3.Api.V1.ChaptersReads.Commands
{
    public static class MarkChapterAsRead
    {
        public sealed record Command(long Chapter, long Story) : IRequest<IActionResult>;

        public class MarkChapterAsReadHandler : IRequestHandler<Command, IActionResult>
        {
            private readonly ApplicationDbContext _context;
            private readonly long? _uid;

            public MarkChapterAsReadHandler(ApplicationDbContext context, IUserService userService)
            {
                _context = context;
                _uid = userService?.User?.GetNumericId();
            }

            public async Task<IActionResult> Handle(Command request, CancellationToken cancellationToken)
            {
                if (_uid is null) return new UnauthorizedResult();

                var (chapter, story) = request;

                var chaptersReadObj = await _context.ChaptersRead
                    .Where(cr => cr.StoryId == story)
                    .Where(cr => cr.UserId == _uid)
                    .FirstOrDefaultAsync(cancellationToken);

                if (chaptersReadObj is null)
                {
                    var result = _context.ChaptersRead.Add(new ChaptersRead
                    {
                        StoryId = story,
                        UserId = (long)_uid,
                        Chapters = new HashSet<long> { chapter }
                    });
                    chaptersReadObj = result.Entity;
                }
                else
                {
                    chaptersReadObj.Chapters.Add(chapter);
                    _context.Entry(chaptersReadObj).State = EntityState.Modified;
                }

                // Save
                try
                {
                    await _context.SaveChangesAsync(cancellationToken);
                    return new OkObjectResult(new { Read = chaptersReadObj.Chapters });
                }
                catch (Exception e)
                {
                    Log.Error(e, "Exception occurred when marking chapter {Chapter} as read by {User}", chapter, _uid);
                    return new ServerErrorResult("Database insert error");
                }
            }
        }
    }
}