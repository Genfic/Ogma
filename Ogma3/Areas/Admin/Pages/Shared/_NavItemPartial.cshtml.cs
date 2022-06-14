#nullable enable

using Humanizer;

namespace Ogma3.Areas.Admin.Pages.Shared;

public class NavItemPartial
{
	public NavItemPartial(string page) => Page = page;

	public NavItemPartial(string page, string text) => (Page, Text) = (page, text);

	public string Page { get; set; }

	private string? _text;
	public string Text
	{
		get => _text ?? Page.Humanize();
		set => _text = value;
	}
}