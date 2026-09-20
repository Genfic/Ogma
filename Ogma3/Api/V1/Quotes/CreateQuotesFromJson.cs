using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Immediate.Validations.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Ogma3.Data;
using Ogma3.Data.Quotes;
using Ogma3.Infrastructure.ServiceRegistrations;
using Ogma3.Services.ModeratorActionService;

namespace Ogma3.Api.V1.Quotes;

using ResponseType = Ok<int>;

[Handler]
[MapGroup<ApiGroup>]
[MapPost("quotes/json")]
[Authorize(AuthorizationPolicies.RequireAdminRole)]
public sealed partial class CreateQuotesFromJson(AppDbContext context, IModeratorActionService moderatorActionService)
{
	internal static void CustomizeEndpoint(IEndpointConventionBuilder endpoint)
		=> endpoint
			.DisableAntiforgery()
			.ProducesValidationProblem();

	private async ValueTask<ResponseType> HandleAsync(
		Query request,
		CancellationToken cancellationToken
	)
	{
		var quotes = request.Quotes
			.Select(q => new Quote
			{
				Body = q.Body,
				Author = q.Author,
			})
			.ToArray();

		context.Quotes.AddRange(quotes);

		await context.SaveChangesAsync(cancellationToken);

		foreach (var quote in quotes)
		{
			moderatorActionService.LogQuoteCreated(quote.Id, quote.Author);
		}

		return TypedResults.Ok(quotes.Length);

	}

	[Validate]
	public sealed partial record Query(QuoteDto[] Quotes) : IValidationTarget<Query>;
}