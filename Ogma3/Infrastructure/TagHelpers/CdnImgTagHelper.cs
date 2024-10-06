using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Ogma3.Infrastructure.TagHelpers;

public sealed class CdnImgTagHelper : TagHelper
{
	public required string Src { get; init; }
	public required int Width { get; init; }
	public required int Height { get; init; }
	public required string Alt { get; init; }
	public bool Eager { get; init; } = false;
	public string? Buster { get; init; } = null;

	public override void Process(TagHelperContext context, TagHelperOutput output)
	{
		var src = string.IsNullOrEmpty(Src)
			? "/img/placeholders/ph-250.png"
			: Src;

		output.Attributes.SetAttribute("width", Width);
		output.Attributes.SetAttribute("height", Height);
		output.Attributes.SetAttribute("alt", Alt);

		if (!string.IsNullOrEmpty(Buster)) src += $"?v={Buster}";

		output.TagName = "img";
		output.Attributes.SetAttribute("src", src);

		if (!Eager)
		{
			output.Attributes.SetAttribute("loading", "lazy");
		}
	}
}