using FluentValidation;
using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Ogma3.Data;
using Ogma3.Data.Reports;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Infrastructure.ServiceRegistrations;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.Reports;

using ReturnType = Results<UnauthorizedHttpResult, BadRequest, Ok<long>>;

[Handler]
[MapPost("api/reports")]
[Authorize]
public static partial class ReportContent
{
	internal static void CustomizeEndpoint(IEndpointConventionBuilder endpoint)
		=> endpoint.RequireRateLimiting(RateLimiting.Reports);

	[UsedImplicitly]
	public sealed record Command(long ItemId, string Reason, EReportableContentTypes ItemType);

	public sealed class CommandValidator : AbstractValidator<Command>
	{
		public CommandValidator() => RuleFor(r => r.Reason)
			.MinimumLength(CTConfig.CReport.MinReasonLength)
			.MaximumLength(CTConfig.CReport.MaxReasonLength);
	}

	private static async ValueTask<ReturnType> HandleAsync(
		Command request,
		ApplicationDbContext context,
		IUserService userService,
		CancellationToken cancellationToken
	)
	{
		if (userService.User?.GetNumericId() is not {} uid) return TypedResults.Unauthorized();

		var report = new Report
		{
			Reason = request.Reason,
			ReporterId = uid,
			ContentType = request.ItemType.ToString(),
		};

		switch (request.ItemType)
		{
			case EReportableContentTypes.Comment:
				report.CommentId = request.ItemId;
				break;
			case EReportableContentTypes.User:
				report.UserId = request.ItemId;
				break;
			case EReportableContentTypes.Story:
				report.StoryId = request.ItemId;
				break;
			case EReportableContentTypes.Chapter:
				report.ChapterId = request.ItemId;
				break;
			case EReportableContentTypes.Blogpost:
				report.BlogpostId = request.ItemId;
				break;
			case EReportableContentTypes.Club:
				report.ClubId = request.ItemId;
				break;
			default:
				return TypedResults.BadRequest();
		}

		context.Reports.Add(report);
		await context.SaveChangesAsync(cancellationToken);

		return TypedResults.Ok(report.Id);
	}
}