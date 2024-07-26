using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
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

	public sealed record Command(long Id, string Body, string Author);
	
	private static async ValueTask<ReturnType> HandleAsync(
		Command request,
		ApplicationDbContext context,
		CancellationToken cancellationToken
	)
	{
		var (id, body, author) = request;

		var res = await context.Quotes
			.Where(q => q.Id == id)
			.ExecuteUpdateAsync(q => q
					.SetProperty(x => x.Body, body)
					.SetProperty(x => x.Author, author),
				cancellationToken);

		return res > 0 ? TypedResults.Ok() : TypedResults.NotFound();
	}
}