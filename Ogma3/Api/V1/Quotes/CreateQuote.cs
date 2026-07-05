using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Immediate.Validations.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Ogma3.Data;
using Ogma3.Data.Quotes;
using Ogma3.Infrastructure.ServiceRegistrations;

namespace Ogma3.Api.V1.Quotes;

using ReturnType = Results<StatusCodeHttpResult, CreatedAtRoute<FullQuoteDto>>;

[Handler]
[MapGroup<ApiGroup>]
[MapPost("quotes")]
[Authorize(AuthorizationPolicies.RequireAdminRole)]
public sealed partial class CreateQuote(ApplicationDbContext context, ILogger<CreateQuote.Handler> logger)
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
		var quote = new Quote
		{
			Body = request.Body,
			Author = request.Author,
		};
		context.Quotes.Add(quote);

		await context.SaveChangesAsync(cancellationToken);

		logger.LogInformation("Quote created at route {Name}", nameof(GetSingleQuote));

		return TypedResults.CreatedAtRoute(
			new FullQuoteDto(quote.Id, quote.Body, quote.Author),
			nameof(GetSingleQuote),
			new { quote.Id }
		);
	}

	[Validate]
	public sealed partial record Command : IValidationTarget<Command>
	{
		[NotEmpty]
		public required string Body { get; init; }
		[NotEmpty]
		public required string Author { get; init; }
	}
}