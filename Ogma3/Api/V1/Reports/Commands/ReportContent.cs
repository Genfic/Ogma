using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ogma3.Data;
using Ogma3.Data.Reports;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.Reports.Commands
{
    public static class ReportContent
    {
        public sealed record Command(long ItemId, string Reason, EReportableContentTypes ItemType) : IRequest<ActionResult<long>>;
        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator() => RuleFor(r => r.Reason).MinimumLength(30).MaximumLength(500);
        }
        
        public class Handler : IRequestHandler<Command, ActionResult<long>>
        {
            private readonly ApplicationDbContext _context;
            private readonly long? _uid;
            
            public Handler(ApplicationDbContext context, IUserService userService)
            {
                _context = context;
                _uid = userService.User?.GetNumericId();
            }

            public async Task<ActionResult<long>> Handle(Command request, CancellationToken cancellationToken)
            {
                if (_uid is null) return new UnauthorizedResult();
                
                var (itemId, reason, itemType) = request;

                var report = new Report
                {
                    Reason = reason,
                    ReporterId = (long)_uid,
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
                        return new BadRequestResult();
                }

                _context.Reports.Add(report);
                await _context.SaveChangesAsync(cancellationToken);

                return new OkObjectResult(report.Id);
            }
        }
    }
}