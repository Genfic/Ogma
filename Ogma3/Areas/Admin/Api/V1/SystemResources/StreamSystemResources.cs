using System.Text.Json;
using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Microsoft.AspNetCore.Authorization;
using Ogma3.Infrastructure.IResults;
using Ogma3.Infrastructure.ServiceRegistrations;
using Ogma3.Services;

namespace Ogma3.Areas.Admin.Api.V1.SystemResources;

[Handler]
[MapGet($"admin/api/system-resources/{nameof(StreamSystemResources)}")]
[Authorize(AuthorizationPolicies.RequireStaffRole)]
public sealed partial class StreamSystemResources(SystemResourceMonitor monitor)
{
	private static readonly TimeSpan Interval = TimeSpan.FromSeconds(1);

	internal static void CustomizeEndpoint(IEndpointConventionBuilder endpoint) => endpoint.ExcludeFromDescription();

	public sealed record Query;

	private ValueTask<ServerSentEventsResult> HandleAsync(Query _, CancellationToken cancellationToken)
		=> new(new ServerSentEventsResult(
			Interval,
			async token => JsonSerializer.Serialize(await monitor.GetSnapshotAsync(token), SystemResourceSnapshotContext.Default.SystemResourceSnapshot),
			cancellationToken)
		);
}