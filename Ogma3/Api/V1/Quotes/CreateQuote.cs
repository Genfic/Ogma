using FluentValidation;
using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Ogma3.Data;
using Ogma3.Data.Quotes;
using Ogma3.Infrastructure.ServiceRegistrations;

namespace Ogma3.Api.V1.Quotes;

using ReturnType = Results<StatusCodeHttpResult, CreatedAtRoute<QuoteDto>>;

[Handler]
[MapPost("api/quotes")]
[Authorize(AuthorizationPolicies.RequireAdminRole)]
public static partial class CreateQuote
{
	internal static void CustomizeEndpoint(IEndpointConventionBuilder endpoint) => endpoint
		.DisableAntiforgery();

	public sealed record Command(string Body, string Author);

	public class CommandValidator : AbstractValidator<Command>
	{
		public CommandValidator()
		{
			RuleFor(q => q.Body).NotEmpty();
			RuleFor(q => q.Author).NotEmpty();
		}
	}
	
	private static async ValueTask<ReturnType> HandleAsync(
		Command request,
		ApplicationDbContext context,
		ILogger<Handler> logger,
		CancellationToken cancellationToken
	)
	{
		var (body, author) = request;
		var quote = new Quote
		{
			Body = body,
			Author = author,
		};
		context.Quotes.Add(quote);

		await context.SaveChangesAsync(cancellationToken);
		
		logger.LogInformation("Quote created at route {Name}", nameof(GetSingleQuote));

		return TypedResults.CreatedAtRoute(
			new QuoteDto(quote.Body, quote.Author),
			nameof(GetSingleQuote),
			new { quote.Id }
		);
	}
}