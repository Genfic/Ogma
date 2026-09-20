using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Immediate.Validations.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Infrastructure.ServiceRegistrations;
using Ogma3.Services.ModeratorActionService;

namespace Ogma3.Api.V1.Faqs;

using ReturnType = Results<NotFound, Ok<long>>;

[Handler]
[MapGroup<ApiGroup>]
[MapDelete("faqs")]
[Authorize(AuthorizationPolicies.RequireAdminRole)]
public sealed partial class DeleteFaq(AppDbContext context, IModeratorActionService moderatorActionService)
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

		var res = await context.Faqs
			.Where(f => f.Id == request.Id)
			.ExecuteDeleteAsync(cancellationToken);

		if (res > 0)
		{
			moderatorActionService.LogFaqDeleted(faq.Id, faq.Question);
		}

		return res > 0 ? TypedResults.Ok(request.Id) : TypedResults.NotFound();
	}

	[Validate]
	public sealed partial record Command(long Id) : IValidationTarget<Command>;
}