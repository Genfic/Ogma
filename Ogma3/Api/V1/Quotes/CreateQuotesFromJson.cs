using System.Text.Json;
using System.Text.Json.Serialization;
using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Ogma3.Data;
using Ogma3.Data.Quotes;
using Ogma3.Infrastructure.ServiceRegistrations;

namespace Ogma3.Api.V1.Quotes;

using ResponseType=Results<BadRequest, Ok<int>>;

[Handler]
[MapPost("api/quotes/json")]
[Authorize(AuthorizationPolicies.RequireAdminRole)]
public static partial class CreateQuotesFromJson
{
	internal static void CustomizeEndpoint(IEndpointConventionBuilder endpoint) => endpoint
		.DisableAntiforgery();

	public sealed record Command(Stream Data);
	
	private static async ValueTask<ResponseType> HandleAsync(Command request, ApplicationDbContext context, CancellationToken cancellationToken)
	{
		var data = await JsonSerializer.DeserializeAsync(request.Data, QuoteJsonContext.Default.QuoteArray, cancellationToken);
		if (data is null) return TypedResults.BadRequest();

		context.Quotes.AddRange(data);

		var insertedRows = await context.SaveChangesAsync(cancellationToken);
		return TypedResults.Ok(insertedRows);

	}
}

[JsonSerializable(typeof(Quote[]))]
[JsonSourceGenerationOptions(JsonSerializerDefaults.Web)]
public partial class QuoteJsonContext : JsonSerializerContext;