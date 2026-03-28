using Humanizer;

namespace Ogma3.Areas.Admin.Pages.Shared;

public sealed class NavItemPartial(string page, string? text = null)
{
	public string Page { get; init; } = page;

	public string Text
	{
		get => field ?? Page.Humanize();
		init;
	} = text;
}