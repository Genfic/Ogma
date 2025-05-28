using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Immediate.Validations.Shared;
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

	[Validate]
	public sealed partial record Command : IValidationTarget<Command>
	{
		public required long ItemId { get; init; }
		[MinLength(CTConfig.Report.MinReasonLength)]
		[MaxLength(CTConfig.Report.MaxReasonLength)]
		public required string Reason { get; init; }
		public required EReportableContentTypes ItemType { get; init; }
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