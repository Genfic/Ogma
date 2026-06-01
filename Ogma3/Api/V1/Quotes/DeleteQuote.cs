using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Immediate.Validations.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Infrastructure.ServiceRegistrations;

namespace Ogma3.Api.V1.Quotes;

using ResponseType = Results<Ok<long>, NotFound>;

[Handler]
[MapDelete("api/quotes/{id:long}")]
[Authorize(AuthorizationPolicies.RequireAdminRole)]
public sealed partial class DeleteQuote(ApplicationDbContext context)
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
		var res = await context.Quotes
			.Where(q => q.Id == request.Id)
			.ExecuteDeleteAsync(cancellationToken);

		return res > 0 ? TypedResults.Ok(request.Id) : TypedResults.NotFound();
	}

	[Validate]
	public sealed partial record Command(long Id) : IValidationTarget<Command>;
}