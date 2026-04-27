using SafeRouting;

namespace Ogma3.Services.SidebarNavService;

public sealed record NavbarItem
{
	public NavbarItem(IPageRouteValues path, string? name = null)
	{
		Path = path;
		Name = name ?? path.PageName.Trim('/');
	}
	public IPageRouteValues Path { get; }
	public string? Name { get; }
}