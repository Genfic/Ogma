using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Immediate.Validations.Shared;
using Markdig;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Infrastructure.Constants;
using Ogma3.Infrastructure.ServiceRegistrations;
using Ogma3.Services.ModeratorActionService;

namespace Ogma3.Api.V1.Faqs;

using ReturnType = Results<NotFound, Ok>;

[Handler]
[MapGroup<ApiGroup>]
[MapPut("faqs")]
[Authorize(AuthorizationPolicies.RequireAdminRole)]
public sealed partial class UpdateFaq(AppDbContext context, IModeratorActionService moderatorActionService)
{
	internal static void CustomizeEndpoint(RouteHandlerBuilder endpoint)
		=> endpoint
			.ProducesValidationProblem();

	private async ValueTask<ReturnType> HandleAsync(
		Command request,
		CancellationToken cancellationToken
	)
	{
		var faq = await context.Faqs
			.Where(f => f.Id == request.Id)
			.Select(f => new { f.Id, f.Question })
			.FirstOrDefaultAsync(cancellationToken);

		if (faq is null)
		{
			return TypedResults.NotFound();
		}

		var rendered = Markdown.ToHtml(request.Answer, MarkdownPipelines.All);

		var res = await context.Faqs
			.Where(f => f.Id == request.Id)
			.ExecuteUpdateAsync(setPropertyCalls: f => f
					.SetProperty(propertyExpression: x => x.Question, request.Question)
					.SetProperty(propertyExpression: x => x.Answer, request.Answer)
					.SetProperty(propertyExpression: x => x.AnswerRendered, rendered),
				cancellationToken);

		if (res > 0)
		{
			moderatorActionService.LogFaqUpdated(faq.Id, faq.Question);
		}

		return res > 0 ? TypedResults.Ok() : TypedResults.NotFound();
	}

	[Validate]
	public sealed partial record Command : IValidationTarget<Command>
	{
		public required long Id { get; init; }
		[NotEmpty]
		public required string Question { get; init; }
		[NotEmpty]
		public required string Answer { get; init; }
	}
}