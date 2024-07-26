using FluentValidation;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Ogma3.Data;
using Ogma3.Data.Reports;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Infrastructure.Mediator.Bases;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.Reports.Commands;

public static class ReportContent
{
	public sealed record Command(long ItemId, string Reason, EReportableContentTypes ItemType) : IRequest<ActionResult<long>>;

	public class CommandValidator : AbstractValidator<Command>
	{
		public CommandValidator() => RuleFor(r => r.Reason).MinimumLength(30).MaximumLength(500);
	}

	public class Handler(ApplicationDbContext context, IUserService userService) : BaseHandler, IRequestHandler<Command, ActionResult<long>>
	{
		
		public async ValueTask<ActionResult<long>> Handle(Command request, CancellationToken cancellationToken)
		{
			if (userService.User?.GetNumericId() is not {} uid) return Unauthorized();

			var (itemId, reason, itemType) = request;

			var report = new Report
			{
				Reason = reason,
				ReporterId = uid,
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

			context.Reports.Add(report);
			await context.SaveChangesAsync(cancellationToken);

			return Ok(report.Id);
		}
	}
}