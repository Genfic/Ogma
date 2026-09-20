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

using ReturnType = Results<Ok, NotFound>;

[Handler]
[MapGroup<ApiGroup>]
[MapPut("quotes")]
[Authorize(AuthorizationPolicies.RequireAdminRole)]
public sealed partial class UpdateQuote(AppDbContext context, IModeratorActionService moderatorActionService)
{
	internal static void CustomizeEndpoint(IEndpointConventionBuilder endpoint)
		=> endpoint
			.DisableAntiforgery()
			.ProducesValidationProblem();

	private async ValueTask<ReturnType> HandleAsync(
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
			.ExecuteUpdateAsync(setPropertyCalls: q => q
					.SetProperty(propertyExpression: x => x.Body, request.Body)
					.SetProperty(propertyExpression: x => x.Author, request.Author),
				cancellationToken);

		if (res > 0)
		{
			moderatorActionService.LogQuoteUpdated(quote.Id, quote.Author);
		}

		return res > 0 ? TypedResults.Ok() : TypedResults.NotFound();
	}

	[Validate]
	public sealed partial record Command : IValidationTarget<Command>
	{
		public required long Id { get; init; }
		[NotEmpty]
		public required string Body { get; init; }
		[NotEmpty]
		public required string Author { get; init; }
	}
}