using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Immediate.Validations.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Infrastructure.ServiceRegistrations;

namespace Ogma3.Api.V1.Quotes;

using ReturnType = Results<Ok, NotFound>;

[Handler]
[MapGroup<ApiGroup>]
[MapPut("quotes")]
[Authorize(AuthorizationPolicies.RequireAdminRole)]
public sealed partial class UpdateQuote(ApplicationDbContext context)
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
		var res = await context.Quotes
			.Where(q => q.Id == request.Id)
			.ExecuteUpdateAsync(setPropertyCalls: q => q
					.SetProperty(propertyExpression: x => x.Body, request.Body)
					.SetProperty(propertyExpression: x => x.Author, request.Author),
				cancellationToken);

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