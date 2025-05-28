using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Immediate.Validations.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Infrastructure.ServiceRegistrations;

namespace Ogma3.Api.V1.Quotes;

using ReturnType=Results<Ok, NotFound>;

[Handler]
[MapPut("api/quotes")]
[Authorize(AuthorizationPolicies.RequireAdminRole)]
public static partial class UpdateQuote
{
	internal static void CustomizeEndpoint(IEndpointConventionBuilder endpoint) => endpoint
		.DisableAntiforgery();

	[Validate]
	public sealed partial record Command : IValidationTarget<Command>
	{
		public required long Id { get; init; }
		[NotEmpty]
		public required string Body { get; init; }
		[NotEmpty]
		public required string Author { get; init; }
	}

	private static async ValueTask<ReturnType> HandleAsync(
		Command request,
		ApplicationDbContext context,
		CancellationToken cancellationToken
	)
	{
		var res = await context.Quotes
			.Where(q => q.Id == request.Id)
			.ExecuteUpdateAsync(q => q
					.SetProperty(x => x.Body, request.Body)
					.SetProperty(x => x.Author, request.Author),
				cancellationToken);

		return res > 0 ? TypedResults.Ok() : TypedResults.NotFound();
	}
}