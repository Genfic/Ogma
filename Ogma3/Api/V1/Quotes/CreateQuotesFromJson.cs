using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Immediate.Validations.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Ogma3.Data;
using Ogma3.Data.Quotes;
using Ogma3.Infrastructure.ServiceRegistrations;

namespace Ogma3.Api.V1.Quotes;

using ResponseType = Ok<int>;

[Handler]
[MapPost("api/quotes/json")]
[Authorize(AuthorizationPolicies.RequireAdminRole)]
public sealed partial class CreateQuotesFromJson(ApplicationDbContext context)
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
		var quotes = request.Quotes.Select(q => new Quote
		{
			Body = q.Body,
			Author = q.Author,
		});

		context.Quotes.AddRange(quotes);

		var insertedRows = await context.SaveChangesAsync(cancellationToken);
		return TypedResults.Ok(insertedRows);

	}

	[Validate]
	public sealed partial record Query(QuoteDto[] Quotes) : IValidationTarget<Query>;
}