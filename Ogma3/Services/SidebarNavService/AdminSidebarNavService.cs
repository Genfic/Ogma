using System.Collections.Frozen;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Routes.Areas.Admin.Pages;
using Index = Routes.Areas.Admin.Pages.Index;

namespace Ogma3.Services.SidebarNavService;

[RegisterScoped]
public sealed class AdminSidebarNavService(EndpointDataSource endpointData, IAuthorizationService authorizationService)
{
	private static readonly FrozenDictionary<string, NavbarItem[]> AllNavbarItems = new Dictionary<string, NavbarItem[]>
	{
		[""] =
		[
			new(Index.Get(), "Dashboard"),
			new(Settings.Get()),
			new(Email.Get(), "Mailer"),
		],
		["Categorization"] =
		[
			new(Tags.Get()),
			new(Ratings.Get()),
		],
		["Content"] =
		[
			new(Quotes.Get()),
			new(Documents_Index.Get(), "Documents"),
			new(ContentBlock.Get()),
		],
		["Users"] =
		[
			new(InviteCodes.Get()),
			new(Roles.Get()),
			new(Users_Index.Get(), "Users"),
			new(Infractions.Get()),
		],
		["Other"] =
		[
			new(ModLog.Get()),
			new(Reports.Get()),
			new(Faq.Get()),
		],
	}.ToFrozenDictionary();

	private static FrozenDictionary<string, IReadOnlyList<IAuthorizeData>>? _policyMap;

	public async Task<IReadOnlyDictionary<string, List<NavbarItem>>> GetAccessibleItems(ClaimsPrincipal user)
	{
		_policyMap ??= endpointData.Endpoints
			.OfType<RouteEndpoint>()
			.Select(ep => new
			{
				PageName = ep.Metadata.GetMetadata<PageActionDescriptor>()?.ViewEnginePath,
				AuthData = ep.Metadata.GetOrderedMetadata<IAuthorizeData>(),
			})
			.Where(x => x.PageName is not null)
			.DistinctBy(x => x.PageName)
			.ToFrozenDictionary(x => x.PageName!, x => x.AuthData, StringComparer.OrdinalIgnoreCase);

		var accessible = new Dictionary<string, List<NavbarItem>>();

		foreach (var (category, items) in AllNavbarItems)
		{
			accessible.TryAdd(category, []);

			foreach (var item in items)
			{
				if (!_policyMap.TryGetValue(item.Path.PageName, out var authData) || authData.Count <= 0)
				{
					accessible[category].Add(item);
					continue;
				}

				foreach (var attr in authData)
				{
					if (attr.Policy is { Length: > 0 })
					{
						var result = await authorizationService.AuthorizeAsync(user, resource: null, policyName: attr.Policy);
						if (result.Succeeded)
						{
							accessible[category].Add(item);
							break;
						}
					}

					if (attr.Roles is { Length: > 0 })
					{
						var roles = attr.Roles.Split(',', StringSplitOptions.TrimEntries);
						if (roles.Any(user.IsInRole))
						{
							accessible[category].Add(item);
							break;
						}
					}
				}
			}

		}

		return accessible.AsReadOnly();
	}
}