using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Immediate.Validations.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Infrastructure.ServiceRegistrations;

namespace Ogma3.Api.V1.Faqs;

using ReturnType = Results<NotFound, Ok<long>>;

[Handler]
[MapDelete("api/faqs")]
[Authorize(AuthorizationPolicies.RequireAdminRole)]
public sealed partial class DeleteFaq(ApplicationDbContext context)
{
	internal static void CustomizeEndpoint(RouteHandlerBuilder endpoint)
		=> endpoint
			.ProducesValidationProblem();

	private async ValueTask<ReturnType> HandleAsync(
		Command request,
		CancellationToken cancellationToken
	)
	{
		var res = await context.Faqs
			.Where(f => f.Id == request.Id)
			.ExecuteDeleteAsync(cancellationToken);

		return res > 0 ? TypedResults.Ok(request.Id) : TypedResults.NotFound();
	}

	[Validate]
	public sealed partial record Command(long Id) : IValidationTarget<Command>;
}