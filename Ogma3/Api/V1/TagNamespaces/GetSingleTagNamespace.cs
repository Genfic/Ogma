using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Immediate.Validations.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.TagNamespaces;
using Ogma3.Infrastructure.ServiceRegistrations;

namespace Ogma3.Api.V1.TagNamespaces;

using ReturnType = Results<Ok<TagNamespaceDto>, NotFound>;

[Handler]
[MapGroup<ApiGroup>]
[MapGet("tagnamespaces/{namespaceId:long}")]
[Authorize(AuthorizationPolicies.RequireAdminRole)]
public sealed partial class GetSingleTagNamespace(AppDbContext context)
{
	internal static void CustomizeEndpoint(IEndpointConventionBuilder endpoint)
		=> endpoint
			.WithName(nameof(GetSingleTagNamespace))
			.ProducesValidationProblem();

	private async ValueTask<ReturnType> HandleAsync(Query request, CancellationToken cancellationToken)
	{
		var tns = await context.TagNamespaces
			.Where(ns => ns.Id == request.NamespaceId)
			.ProjectToDto()
			.FirstOrDefaultAsync(cancellationToken);

		return tns is null ? TypedResults.NotFound() : TypedResults.Ok(tns);
	}

	[Validate]
	public sealed partial record Query(long NamespaceId) : IValidationTarget<Query>;
}