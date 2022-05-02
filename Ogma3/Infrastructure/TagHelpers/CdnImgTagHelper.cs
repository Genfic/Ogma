#nullable enable


using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Ogma3.Infrastructure.TagHelpers;

public class CdnImgTagHelper : TagHelper
{
	public string Src { get; set; } = null!;
	public int? Width { get; set; }
	public int? Height { get; set; }
	public bool Eager { get; set; } = false;
	public string? Buster { get; set; } = null;

	public override void Process(TagHelperContext context, TagHelperOutput output)
	{
		var src = string.IsNullOrEmpty(Src)
			? "/img/placeholders/ph-250.png"
			: Src;

		if (Width.HasValue) output.Attributes.SetAttribute("width", Width ?? Height);
		if (Height.HasValue) output.Attributes.SetAttribute("height", Height ?? Width);

		if (!string.IsNullOrEmpty(Buster)) src += $"?v={Buster}";

		output.TagName = "img";
		output.Attributes.SetAttribute("src", src);

		if (!Eager)
		{
			output.Attributes.SetAttribute("loading", "lazy");
		}
	}
}