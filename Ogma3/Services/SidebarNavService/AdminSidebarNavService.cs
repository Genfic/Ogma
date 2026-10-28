using System.Collections.Frozen;
using System.Security.Claims;
using Immediate.Injections.Shared;
using Microsoft.AspNetCore.Authorization;
using Routes.Areas.Admin.Pages;
using Utils.Extensions;
using Index = Routes.Areas.Admin.Pages.Index;

namespace Ogma3.Services.SidebarNavService;

[RegisterScoped]
public sealed class AdminSidebarNavService(SidebarNavDataCache navData, IAuthorizationService authorizationService)
{
	private static readonly FrozenDictionary<string, NavbarItem[]> AllNavbarItems = new Dictionary<string, NavbarItem[]>
	{
		[""] =
		[
			new(Index.Get(), "Dashboard"),
			new(Settings.Get()),
			new(Email.Get(), "Mailer"),
			new(Notifications.Get(), "Notifications"),
		],
		["Categorization"] =
		[
			new(Tags.Get()),
			new(TagNamespaces.Get()),
			new(Ratings.Get()),
		],
		["Content"] =
		[
			new(Quotes.Get()),
			new(Documents_Index.Get(), "Documents"),
			new (News_Index.Get(), "News"),
			new(ContentBlock.Get()),
		],
		["Users"] =
		[
			new(InviteCodes.Get()),
			new(Roles.Get()),
			new(Users_Index.Get(), "Users"),
			new(Infractions.Get()),
			new(Subscriptions.Get()),
		],
		["Other"] =
		[
			new(SystemStats.Get(), "System resources"),
			new(Reports.Get()),
			new(ModLog.Get()),
			new(Faq.Get()),
		],
	}.ToFrozenDictionary();

	public async Task<IReadOnlyDictionary<string, List<NavbarItem>>> GetAccessibleItems(ClaimsPrincipal user)
	{
		var accessible = new Dictionary<string, List<NavbarItem>>();

		foreach (var (category, items) in AllNavbarItems)
		{
			accessible.TryAdd(category, []);
			await AddAccessibleItemsAsync(user, category, items, accessible);
		}

		return accessible.AsReadOnly();
	}

	private async Task AddAccessibleItemsAsync(
		ClaimsPrincipal user,
		string category,
		IEnumerable<NavbarItem> items,
		Dictionary<string, List<NavbarItem>> accessible)
	{
		foreach (var item in items)
		{
			if (await IsItemAccessibleAsync(user, item))
			{
				accessible[category].Add(item);
			}
		}
	}

	private async Task<bool> IsItemAccessibleAsync(ClaimsPrincipal user, NavbarItem item)
	{
		if (!navData.PolicyMap.TryGetValue(item.Path.PageName, out var authData) || authData.Count <= 0)
		{
			return true;
		}

		foreach (var attr in authData)
		{
			if (await HasAccessAsync(user, attr))
			{
				return true;
			}
		}

		return false;
	}

	private async Task<bool> HasAccessAsync(ClaimsPrincipal user, IAuthorizeData attr)
	{
		if (attr.Policy is { Length: > 0 })
		{
			var result = await authorizationService.AuthorizeAsync(user, null, attr.Policy);
			if (result.Succeeded)
			{
				return true;
			}
		}

		if (attr.Roles is not { Length: > 0 })
		{
			return false;
		}

		var roles = attr.Roles.Split(',', StringSplitOptions.TrimEntries);
		return !roles.None(user.IsInRole);
	}
}