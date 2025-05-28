using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Immediate.Validations.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Infrastructure.ServiceRegistrations;

namespace Ogma3.Api.V1.Quotes;

using ResponseType=Results<Ok<long>, NotFound>;

[Handler]
[MapDelete("api/quotes/{id:long}")]
[Authorize(AuthorizationPolicies.RequireAdminRole)]
public static partial class DeleteQuote
{
	internal static void CustomizeEndpoint(IEndpointConventionBuilder endpoint) => endpoint
		.DisableAntiforgery();

	[Validate]
	public sealed partial record Command(long Id) : IValidationTarget<Command>;

	private static async ValueTask<ResponseType> HandleAsync(
		Command request,
		ApplicationDbContext context,
		CancellationToken cancellationToken
	)
	{
		var res = await context.Quotes
			.Where(q => q.Id == request.Id)
			.ExecuteDeleteAsync(cancellationToken);

		return res > 0 ? TypedResults.Ok(request.Id) : TypedResults.NotFound();
	}

}