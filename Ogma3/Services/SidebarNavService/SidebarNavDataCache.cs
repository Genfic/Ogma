using System.Collections.Frozen;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ogma3.Services.SidebarNavService;

[RegisterSingleton]
[UsedImplicitly]
public sealed class SidebarNavDataCache(EndpointDataSource endpointData)
{
	public FrozenDictionary<string, IReadOnlyList<IAuthorizeData>> PolicyMap { get; } = endpointData.Endpoints
		.OfType<RouteEndpoint>()
		.Select(ep => new
		{
			PageName = ep.Metadata.GetMetadata<PageActionDescriptor>()?.ViewEnginePath,
			AuthData = ep.Metadata.GetOrderedMetadata<IAuthorizeData>(),
		})
		.Where(x => x.PageName is not null)
		.DistinctBy(x => x.PageName)
		.ToFrozenDictionary(x => x.PageName!, x => x.AuthData, StringComparer.OrdinalIgnoreCase);
}