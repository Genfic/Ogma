using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Immediate.Validations.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Infrastructure.ServiceRegistrations;
using Ogma3.Services.ModeratorActionService;

namespace Ogma3.Api.V1.Quotes;

using ResponseType = Results<Ok<long>, NotFound>;

[Handler]
[MapGroup<ApiGroup>]
[MapDelete("quotes/{id:long}")]
[Authorize(AuthorizationPolicies.RequireAdminRole)]
public sealed partial class DeleteQuote(AppDbContext context, IModeratorActionService moderatorActionService)
{
	internal static void CustomizeEndpoint(IEndpointConventionBuilder endpoint)
		=> endpoint
			.DisableAntiforgery()
			.ProducesValidationProblem();

	private async ValueTask<ResponseType> HandleAsync(
		Command request,
		CancellationToken cancellationToken
	)
	{
		var quote = await context.Quotes
			.Where(q => q.Id == request.Id)
			.Select(q => new { q.Id, q.Author })
			.FirstOrDefaultAsync(cancellationToken);

		if (quote is null)
		{
			return TypedResults.NotFound();
		}

		var res = await context.Quotes
			.Where(q => q.Id == request.Id)
			.ExecuteDeleteAsync(cancellationToken);

		if (res > 0)
		{
			moderatorActionService.LogQuoteDeleted(quote.Id, quote.Author);
		}

		return res > 0 ? TypedResults.Ok(request.Id) : TypedResults.NotFound();
	}

	[Validate]
	public sealed partial record Command(long Id) : IValidationTarget<Command>;
}